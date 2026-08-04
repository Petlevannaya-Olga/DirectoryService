using Primitives;

namespace DirectoryService.Application.Locations;

public static partial class LocationErrors
{
    public static Error AddressConflict()
    {
        return CommonErrors.Conflict(
            "location.address.conflict",
            "Локация с таким адресом уже существует");
    }

    public static Error NameConflict()
    {
        return CommonErrors.Conflict(
            "location.name.conflict",
            "Локация с таким именем уже существует");
    }
}