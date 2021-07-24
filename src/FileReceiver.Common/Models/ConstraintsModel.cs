using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using FileReceiver.Common.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FileReceiver.Common.Models
{
    public class ConstraintsModel
    {
        private readonly Dictionary<ConstraintType, string> _constraints;

        public ConstraintsModel()
        {
            _constraints = new Dictionary<ConstraintType, string>();
        }

        public ConstraintsModel(string constraintsAsJson)
        {
            _constraints = JsonConvert.DeserializeObject<Dictionary<ConstraintType, string>>(constraintsAsJson);
        }

        public ConstraintsModel(IDictionary<ConstraintType, string> constraints)
        {
            _constraints = new Dictionary<ConstraintType, string>(constraints);
        }

        public ReadOnlyDictionary<ConstraintType, string> Parameters => new(_constraints);
        public string ConstraintsAsJson => JsonConvert.SerializeObject(_constraints);

        public string GetConstraint(ConstraintType parameter)
        {
            return _constraints[parameter] ?? String.Empty;
        }

        public void AddConstraint(ConstraintType parameter, [NotNull] string value)
        {
            _constraints.Add(parameter, value);
        }

        public void UpdateParameter(ConstraintType parameter, [NotNull] string value)
        {
            _constraints[parameter] = value;
        }

        public override string ToString()
        {
            return ConstraintsAsJson;
        }
    }
}
