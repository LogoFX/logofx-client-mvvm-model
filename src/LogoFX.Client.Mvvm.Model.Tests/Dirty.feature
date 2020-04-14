Feature: Dirty
	In order to support various client app scenarios
	As an app developer
	I want the framework to manage dirty object state properly

Scenario: Initially created simple editable model is not marked as dirty
	When The simple editable model is created with valid name
	Then The simple editable model is not marked as dirty

Scenario: Making simple editable model dirty results in model which is marked as dirty
	When The simple editable model is created with valid name
	And The simple editable model is made dirty
	Then The simple editable model is marked as dirty

Scenario: Setting property to invalid value to a valid simple editable model results in model which is marked as dirty
	When The simple editable model is created with valid name
	And The simple editable model is updated with invalid value for property
	Then The simple editable model is marked as dirty

Scenario: Initially created composite editable model is not marked as dirty
	When The composite editable model is created
	Then The composite editable model is not marked as dirty

Scenario: Setting inner property value to invalid value to a valid composite editable model results in model which is marked as dirty
	When The composite editable model is created
	And The composite editable model is updated with invalid value for inner property value
	Then The composite editable model is marked as dirty

Scenario: Resetting internal model should raise dirty notification
	When The composite editable model is created
	And The internal model is reset
	Then The dirty notification should be raised
