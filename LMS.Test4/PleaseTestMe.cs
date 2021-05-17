using System;

namespace LMS.Test4
{
    internal class PleaseTestMe
    {
        private Microsoft.Extensions.Logging.ILogger<PleaseTestMe> @object;

        public PleaseTestMe(Microsoft.Extensions.Logging.ILogger<PleaseTestMe> @object)
        {
            this.@object = @object;
        }

        internal void RunMe()
        {
            throw new NotImplementedException();
        }
    }
}