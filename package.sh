dotnet build --configuration Release src/k8s-controller-sdk/k8s-controller-sdk.csproj

dotnet pack --configuration Release --output ./bin src/k8s-controller-sdk/k8s-controller-sdk.csproj