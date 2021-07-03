# logofx-client-mvvm-model
Model objects with various aspects, including property change notification, hierarchical change tracking, error management, undo-redo stack capabilities.

<img src=https://ci.appveyor.com/api/projects/status/github/logofx/logofx-client-mvvm-model>

[![CodeFactor](https://www.codefactor.io/repository/github/logofx/logofx-client-mvvm-model/badge/master)](https://www.codefactor.io/repository/github/logofx/logofx-client-mvvm-model/overview/master)

<img src=https://img.shields.io/nuget/dt/LogoFX.Client.Mvvm.Model>

[Living documentation](https://ci.appveyor.com/api/projects/LogoFX/logofx-client-mvvm-model/artifacts/src/LogoFX.Client.Mvvm.Model.Specs/bin/LivingDoc.html)

[Get package](https://www.nuget.org/packages/LogoFX.Client.Mvvm.Model/)

Below you can find examples of most frequent use cases:

## Property notification

```csharp
using LogoFX.Client.Mvvm.Model;

public class MyModel : Model<int>
{
  private int _myField;
  public int MyProperty
  {
    get
    {
      return _myField;
    }
    set
    {
      _myField = value;
      NotifyOfPropertyChange();
    }
  }
}
```

In this case you get out-of-the-box support for property change notification without the need of explicit property name mentioning.

## Validation and Errors

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using LogoFX.Client.Mvvm.Model;

public class NumberValidation : ValidationAttribute
    {
        public NumberValidation()
        {
            Minimum = int.MinValue;
            Maximum = int.MaxValue;
        }

        public int Minimum { get; set; }

        public int Maximum { get; set; }

        protected override ValidationResult IsValid(object value, 
                              ValidationContext validationContext)
        {
            var number = (int) value;

            if (number < Minimum || number > Maximum)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
    
internal sealed class WarehouseItem : Model<Guid>, IWarehouseItem
    {
        public WarehouseItem(
            string kind, 
            double price, 
            int quantity)
        {
            Id = Guid.NewGuid();
            _kind = kind;
            _price = price;
            _quantity = quantity;            
        }

        private string _kind;        
        public string Kind
        {
            get { return _kind; }
            set
            {                
                _kind = Value;
                NotifyOfPropertyChange();
            }
        }

        private double _price;        
        public double Price
        {
            get { return _price;}
            set
            {
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => TotalCost);
            }
        }

        private int _quantity;
        [NumberValidation(Minimum = 1, ErrorMessage = "Quantity must be positive.")]
        public int Quantity
        {
            get { return _quantity; }
            set
            {                
                _quantity = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => TotalCost);
            }
        }        

        public double TotalCost
        {
            get { return _quantity*_price; }
        }
    }
```

In this case when the value of ```Quantity``` is modified the validation execution mechanism is applied and all respective attributes are visited. The model itself has property ```Errors``` which is immediately populated with the resulting error messages if any. The property ```HasError``` will also reflect the current state of the errors inside the model.

In addition to client-side validation there is also ability to manage server-side validation and store the respective errors in the underlying model:

```csharp
//...
var model = new MyModel();
var externalErrors = GetExternalErrors(myModel);
foreach(var error in externalErrors)
{
  model.SetError(error.Message, error.Property);
}
```

The "external" errors are displayed along with the others inside the `Errors` property.

By default the indexed error property and the single `Error` property return the list of errors as a single value, separated by new line. You can override this behaviour and supply your own implementation of errors presentation

```csharp
using System.Collections.Generic;
using System.Linq;
using LogoFX.Client.Mvvm.Model;

public class MyModel : Model<Guid>
{
  protected override string CreateErrorsPresentation(IEnumerable<string> errors)
  {
    return errors.OrderBy(t => t.Length).Join(";");
  }
}

```

