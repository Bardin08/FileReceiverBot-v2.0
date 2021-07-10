using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using FileReceiver.Common.Enums;

using Newtonsoft.Json;

namespace FileReceiver.Common.Models
{
    public class TransactionDataModel
    {
        private readonly Dictionary<TransactionDataParameter, object> _parameters;

        public TransactionDataModel()
        {
            _parameters = new Dictionary<TransactionDataParameter, object>();
        }

        public TransactionDataModel(string parametersAsJson)
        {
            _parameters = JsonConvert.DeserializeObject<Dictionary<TransactionDataParameter, object>>(parametersAsJson);
        }

        public TransactionDataModel(Dictionary<TransactionDataParameter, object> parameters)
        {
            _parameters = parameters;
        }

        public ReadOnlyDictionary<TransactionDataParameter, object> Parameters => new(_parameters);
        public string ParametersAsJson => JsonConvert.SerializeObject(_parameters);

        public void AddDataPiece(TransactionDataParameter parameter, [NotNull] object value)
        {
            _parameters.Add(parameter, value);
        }

        public void UpdateParameter(TransactionDataParameter parameter, [NotNull] object value)
        {
            _parameters[parameter] = value;
        }

        public override string ToString()
        {
            return ParametersAsJson;
        }
    }
}
