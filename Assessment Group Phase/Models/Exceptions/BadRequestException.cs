using System;

namespace Assessment.Group.Phase.Models.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message = null) : base(message)
        {
        }
    }
}
