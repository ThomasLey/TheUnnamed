@description('Name for the container group')
param name string = 'ci-ebookstore'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Container image to deploy. Should be of the form repoName/imagename:tag for images stored in public Docker Hub, or a fully qualified URI for other registries. Images from private registries require additional registry credentials.')
param image string = 'thomasley/theunnamed:latest'

@description('Port to open on the container and the public IP address.')
param port int = 443

@description('The number of CPU cores to allocate to the container.')
param cpuCores int = 1

@description('The amount of memory to allocate to the container in gigabytes.')
param memoryInGb int = 2

@description('The behavior of Azure runtime if container has stopped.')
@allowed([
  'Always'
  'Never'
  'OnFailure'
])
param restartPolicy string = 'Never'

@secure()
param securePassword string = getSecret(subscription().subscriptionId, resourceGroup().name, 'kv-eGov-sonarqube', 'ebookstorecertpwd')
@secure()
param secureClientId string = getSecret(subscription().subscriptionId, resourceGroup().name, 'kv-eGov-sonarqube', 'ebookstoreclientid')

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2021-09-01' = {
  name: name
  location: location
  tags: {
      maintainer: 'Thomas Ley'
      Purpose: 'TheUnnamedPoc'
  }
  properties: {
    containers: [
      {
        name: name
        properties: {
          image: image
          ports: [
            {
              port: port
              protocol: 'TCP'
            }
          ]
          environmentVariables: [
            {
              name: 'AzureAD__ClientSecret'
              value: secureClientId
            }
            {
              name: 'ASPNETCORE_URL'
              value: 'https://+:443;https://+:443'
            }
            {
              name: 'DatabaseConfiguration__ConnectionString'
              value: '/data/db'
            }
            {
              name: 'StorageConfiguration__ConnectionString'
              value: '/data/files'
            }
            {
              name: 'ASPNETCORE_Kestrel__Certificates__Default__Password'
              value: securePassword
            }
            {
              name: 'ASPNETCORE_Kestrel__Certificates__Default__Path'
              value: '/https/certificate.pfx'
            }
          ]
          resources: {
            requests: {
              cpu: cpuCores
              memoryInGB: memoryInGb
            }
          }
        }
      }
    ]
    osType: 'Linux'
    restartPolicy: restartPolicy
    ipAddress: {
      type: 'Public'
      ports: [
        {
          port: port
          protocol: 'TCP'
        }
      ]
    }
  }
}

output containerIPv4Address string = containerGroup.properties.ipAddress.ip
