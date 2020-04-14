Feature: Client Model Validation
	In order to support various client app scenarios
	As an app developer
	I want to get notifications about model validity

Scenario: Valid test value object should have no errors
	When The simple test value object is created with name 'valid name'
	Then The simple test value object has no errors

Scenario: Invalid test value object should have errors
	When The simple test value object is created with name 'in$valid name'
	Then The simple test value object has errors
