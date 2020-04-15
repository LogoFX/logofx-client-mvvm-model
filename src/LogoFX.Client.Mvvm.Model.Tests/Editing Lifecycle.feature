Feature: Editing Lifecycle
	In order to develop rich functionl apps faster
	As an app developer
	I want the framework to support standard editing lifecycle

Scenario: Setting property to invalid value to a valid editable model and then cancelling the changes should result in model with no errors
	When The editable model is created with valid title
	And The editable model is updated with invalid value for property
	And The editable model changes are cancelled
	Then The editable model has no errors

Scenario: Setting external error to a valid editable model after manual property update and then cancelling the changes should result in model with no errors
	When The editable model is created with valid title
	And The editable model is updated with value 'Mr.' for property
	And The editable model is updated with external error
	And The editable model changes are cancelled
	Then The editable model has no errors

Scenario: Setting external error to a valid editable model before manual property update and cancelling the changes after the manual property update should result in model with errors
	When The editable model is created with valid title
	And The editable model is updated with external error
	And The editable model is updated with value 'Mr.' for property
	And The editable model is cleared from external errors
	And The editable model changes are cancelled
	Then The editable model has errors
