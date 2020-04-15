Feature: Undo Redo
	In order to write rich functional apps faster
	As an app developer
	I want the framework to support undo-redo functionality

Scenario: Calling undo once after two editing operations restores the model to previous state and marks it as dirty
	When The simple editable model with undo-redo is created with valid name
	And The name is updated to 'NameOne'
	And The name is updated to 'NameTwo'
	And The last operation for simple editable model with undo-redo is undone
	Then The name should be 'NameOne'
	And The simple editable model with undo-redo is marked as dirty

Scenario: Calling undo twice after two editing operations restores the model to initial state and marks it as not dirty
	When The simple editable model with undo-redo is created with valid name
	And The name is updated to 'NameOne'
	And The name is updated to 'NameTwo'
	And The last operation for simple editable model with undo-redo is undone
	And The last operation for simple editable model with undo-redo is undone
	Then The name should be identical to the valid name
	And The simple editable model with undo-redo is not marked as dirty

Scenario: Calling undo then redo once after two editing operations results in the model with latest state and marks it as dirty
	When The simple editable model with undo-redo is created with valid name
	And The name is updated to 'NameOne'
	And The name is updated to 'NameTwo'
	And The last operation for simple editable model with undo-redo is undone
	And The last operation for simple editable model with undo-redo is redone
	Then The name should be 'NameTwo'
	And The simple editable model with undo-redo is marked as dirty
