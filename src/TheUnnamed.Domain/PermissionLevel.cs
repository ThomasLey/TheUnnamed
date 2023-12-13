namespace TheUnnamed.Domain;

[Flags]
public enum PermissionLevel
{
    Read,
    Download,
    ContributeMetadata,
    ContributeBinaryData,
    Owner
}