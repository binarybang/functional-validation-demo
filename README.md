# Functional validation demo

## Summary

This is a demo of a validation approach based on [LanguageExt](https://github.com/louthy/language-ext) library 
and its `Validation` type in comparison to the classic approach which makes use of FluentValidation
and some form of mapping. \
The demo also uses [Vogen](https://github.com/SteveDunn/Vogen) for one of implemented validation variations.

## Usage
- Build solution to start the API service
- Check `src/WebApi/FunctionalValidation.http` file for request samples

## Structure

Solution contains two variations of domain / application layer models
- lax: the constraints added by data structures are kept to a minimum 
  - primitives for most properties
  - nullable reference types
- strict: constraints make use of LanguageExt's `Option` and "value object" types generated with Vogen.

These models are used to demonstrate the following cases using the endpoints in the `InsurancesController`:
- classic approach with FluentValidation \
IValidator is used to apply validation rules to the request, and then the request is mapped to the app layer model.
- same approach, but with incorrect validation setup \
IValidator is not involved but mapping is the same which leads to errors 
- `Validation`-based approach with the lax model \
In order to adjust the result to the model constraints we "downgrade" the results we get with validation a bit.
- `Validation`-based approach with the strict model \
No downgrade needed if we allow usage of LanguageExt types in inner layers.
