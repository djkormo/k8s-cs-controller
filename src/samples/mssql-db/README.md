![Docker](https://github.com/sebagomez/k8s-cs-controller/workflows/Docker/badge.svg)
## Microsoft SQL Server Database operator

### Definition

This is a controller for a newly defined CustomResourceDefinition that lets you create or delete (drop) databases from s SQL Server pod running in your Kubernetes cluster.

```yaml
apiVersion: apiextensions.k8s.io/v1beta1
kind: CustomResourceDefinition
metadata:
  name: mssqldbs.samples.k8s-cs-controller
spec:
  group: samples.k8s-cs-controller
  version: v1
  subresources:
    status: {}
  scope: Namespaced
  names:
    plural: mssqldbs
    singular: mssqldb
    kind: MSSQLDB
  validation:
    openAPIV3Schema:
      type: object
      description: "A Microsoft SQLServer Database"
      properties:
        spec:
          type: object
          properties:
            dbname:
              type: string
            config:
              type: string
            data:
              type: string
          required: ["dbname","config", "data"]
```

This CRD has three properties, `dbname`, `config`, and `data`. All three of them are strings, but they all have different semantics.  

- `dbname` holds the name of the Database that will be added/delete to the SQL Server instance.
- `config` is the name of a [ConfigMap](https://kubernetes.io/docs/concepts/configuration/configmap/) with a property called `instance`. That's where the name of the [Service](https://kubernetes.io/docs/concepts/services-networking/service/) related to the SQL Server pod is listening.
- `data` is also an indirection, but in this case to a [Secret](https://kubernetes.io/docs/concepts/configuration/secret/) that holds both the user (`userid`) and password (`password`) to the SQL Server instance.

As you can see, these are mandatory for the controller to successfully communicate to the SQL Server instance.

So, a typical yaml for my new Custom Resource will lool like this

```yaml
apiVersion: "samples.k8s-cs-controller/v1"
kind: MSSQLDB
metadata:
  name: db1
spec:
  dbname: MyFirstDB
  config: mssql-config
  data: mssql-data 
```

This yaml will create (or delete) an object of kind MSSQLDB, named db1 with the properties mentioned above. In this case, a ConfigMap called `mssql-config` and a Secret called `mssql-data` must exist.

### Implementation

If we first apply the first file (CustomResourceDefinition) and we then apply the second one, we'll see that Kubernetes successfully creates the object.

> kubectl apply -f .\db1.yaml  
> mssqldb.samples.k8s-cs-controller/db1 created

But nothing actually happens other than the API-Server saving the data in the cluster's etcd database. We need to do something that "listens" for our newly created definition and eventually would create or delete databases.

#### Base class

We need to create a class that represents our definition. For that purpose, the SDK provides a class called `BaseCRD` which is where your class will inherit from. Also, you must create a spec class that will hold the properties defined in your Custom Resource. In my case, this is what they look like.

```cs
public class MSSQLDB : BaseCRD
{
	public MSSQLDB() :
		base("samples.k8s-cs-controller", "v1", "mssqldbs", "mssqldb")
	{ }

	public MSSQLDBSpec Spec { get; set; }
}

public class MSSQLDBSpec
{
	public string DBName { get; set; }

	public string Config { get; set; }

	public string Data { get; set; }
}
```

Keep in mind the strings you must pass over the base class' constructor. These are the same values defined in the CustomeResourceDefinition file.

Then you need to create the class that will be actually creating or deleting the databases. For this purpose, create a class that implements the IOperationHAndler<T>, where T is your implementation of the `BaseCRD`,  in my case `MSSQLDB`.

```cs
public interface IOperationHandler<T> where T : BaseCRD
{
	Task OnAdded(Kubernetes k8s, T crd);

	Task OnDeleted(Kubernetes k8s, T crd);

	Task OnUpdated(Kubernetes k8s, T crd);

	Task OnBookmarked(Kubernetes k8s, T crd);

	Task OnError(Kubernetes k8s, T crd);

	Task CheckCurrentState(Kubernetes k8s);
}
```
The implementation is pretty straight forward, you need to implement the *OnAction* methods. These methods are the ones that will communicate with the SQL Server instance and will create or delete the databases. So whenever somebody uses `kubectl` to create, apply or delete an object, these methods will be called.

But what happens if somebody or something connects to your SQL Server instance and deletes the databases? Here's where the `CheckCurrentState` method comes into play. This method, in my case, is checking every 5 seconds if the MSSQLDB objects created in my cluster are actually created as databases in the SQL Server instance. If they are not, it will try to recreate them.

### Start your engines!

Ok, now it's time to start and try everything.

In my case it's a .NET Core console application where I start the controller. (I've also seen ASP.NET Hosted Services)

```cs
static void Main(string[] args)
{
	MSSQLDBOperationHandler handler = new MSSQLDBOperationHandler();
	Controller<MSSQLDB> controller = new Controller<MSSQLDB>(new MSSQLDB(), handler);
	controller.SatrtAsync(new System.Threading.CancellationToken());
	handler.CheckCurrentState(controller.Kubernetes);

	Console.WriteLine("Press <enter> to quit...");
	Console.ReadLine();
}
```
Here you can see that I first create the handler and pass it over to the controller instance. This `Controller` is given by the SDK and it's the one checking on the objects created by the kubernetes-apiserver. I then start the controller, the handler for the current state, and that's it!.

### Take it for a spin

Start your console application and see what happens.

```
Press <enter> to quit...
mssql_db.MSSQLDB db1 Added on Namespace default
DATABASE MyFirstDB will be ADDED
DATABASE MyFirstDB successfully created!
mssql_db.MSSQLDB db1 Deleted on Namespace default
DATABASE MyFirstDB will be DELETED!
DATABASE MyFirstDB successfully deleted!
mssql_db.MSSQLDB db1 Added on Namespace default
DATABASE MyFirstDB will be ADDED
DATABASE MyFirstDB successfully created!
Database MyFirstDB was not found!
DATABASE MyFirstDB will be ADDED
DATABASE MyFirstDB successfully created!
```

Here's the log of the execution. The first thing I did was created the first db running:

`kubectl apply -f .\db1.yaml`

I then deleted the object, and the database was successfully created:

`kubectl delete -f .\db1.yaml`

I then created it again, and connected to the pod running the SQL Server and dropped the MyFirstDB database, thus, you see that `Database MyFirstDB was not found!` message.

Also, in the log shown above, you'll notice some messages seem to have the same info, but they actually come from two sources. One from the controller engine itself (from inside the SDK) and some form my own MSSQLDB implementation.

### Run it in your container (ACTUALLY, STILL NOT WORKING)

This msqldb controller is also available as a Docker image you can use in your cluster. 

Spin a pod with the following command

`kubectl run mssqldb --image=sebagomez/k8s-mssqldb`

