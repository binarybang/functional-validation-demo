using LanguageExt.Common;

using ValidationApproach.Functional.ErrorsWithPath.Errors;

namespace ErrorsWithPath.Errors;

public record ValidationError(ValuePath Path, string Message)
  : Expected(Message, 1, Option<Error>.None);

public record LengthOutOfBounds(ValuePath Path, int Min, int Max) 
  : ValidationError(Path, $"Length should be between {Min} and {Max}.");

public record FieldIsRequired(ValuePath Path, Option<string> ValueDescription) 
  : ValidationError(Path, $"{ValueDescription.IfNone("Field")} needs to be specified");

public record DateOutOfBounds(ValuePath Path, DateOnly MinDate, DateOnly MaxDate, DateOnly Specified) 
  : ValidationError(Path, $"Date should be between {MinDate} and {MaxDate}, {Specified} is specified");

public record YearOutOfBounds(ValuePath Path, int MinYear, int MaxYear, int Specified)
  : ValidationError(Path, $"Year should be between {MinYear} and {MaxYear}, {Specified} is specified");

public record StartDateInThePast(ValuePath Path, DateOnly SpecifiedDate)
  : ValidationError(Path, $"Start date should not be in the past, but {SpecifiedDate} is specified");


public static class ErrorExtensions {
  public static Seq<ValidationError> InvalidSeq<T>(this T error) where T : ValidationError {
    return [error];
  }
}