using Offr.Text;

namespace Offr.Users
{
    public interface IUserProvider
    {
        User FromPointer(IUserPointer userPointer);
    }
}