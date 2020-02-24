using System;
using System.Collections.Generic;


namespace R5T.Rome
{
    public interface ISecretsFileNamesProvider
    {
        IEnumerable<string> GetSecretsFileNames();
    }
}
