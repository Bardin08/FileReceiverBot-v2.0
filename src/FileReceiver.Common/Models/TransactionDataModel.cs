using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using FileReceiver.Common.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public TransactionDataModel(IDictionary<TransactionDataParameter, object> parameters)
        {
            _parameters = new Dictionary<TransactionDataParameter, object>(parameters);
        }

        public ReadOnlyDictionary<TransactionDataParameter, object> Parameters => new(_parameters);
        public string ParametersAsJson => JsonConvert.SerializeObject(_parameters);

        public object GetDataPiece(TransactionDataParameter parameter)
        {
            return _parameters[parameter] ?? new object();
        }

        public T GetDataPiece<T>(TransactionDataParameter parameter) where T: new()
        {
            // As complex objects stores as a JSON this check is required 
            try
            {
                var parameterValueAsJson = _parameters[parameter].ToString() ?? "";
                if (JToken.Parse(parameterValueAsJson) is { } token)
                {
                    return JsonConvert.DeserializeObject<T>(parameterValueAsJson);
                }
            }
            catch
            {
                // ignored
            }

            return (T)_parameters[parameter] ?? new T();
        }
        
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
