using System;

namespace Assessment.Group.Phase.Models.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message = null) : base(message)
        {
        }
    }
}
