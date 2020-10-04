read_dom () {
    local IFS=\>
    read -d \< ENTITY CONTENT
}

while read_dom; do
    if [[ $ENTITY = "Version" ]]; then
        echo $CONTENT
        exit
    fi
done < ./src/k8s-controller-sdk.csproj