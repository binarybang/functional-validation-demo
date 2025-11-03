using System.Linq.Expressions;

namespace ValidationApproach.Functional.ErrorsWithPath.Errors;

public record ValuePath {
  private readonly List<ValuePathSegment> _segments;

  public static ValuePath Root => new([]);

  public static ValuePath FromString(string pathString) => new([new ValuePathSegment.Raw(pathString)]);

  private ValuePath(List<ValuePathSegment> segments) {
    _segments = segments;
  }

  public ValuePath Combine<T>(Expression<Func<T, object>> pathExpression) {
    return new ValuePath([
      .._segments,
      new ValuePathSegment.ExpressionBased<T>(pathExpression)
    ]);
  }
  
  public ValuePath Combine(string path) {
    return new ValuePath([
      .._segments,
      new ValuePathSegment.Raw(path)
    ]);
  }
  
  public string FullPath() {
    return string.Join(".", _segments.Select(MapSegmentToString));
  }

  /// <summary>
  /// Overridden implementation that checks de-facto equivalence of full path
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public virtual bool Equals(ValuePath? other) {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;
    return FullPath() == other.FullPath();
  }

  /// <summary>
  /// Overridden implementation that is based on of full path
  /// </summary>
  /// <returns></returns>
  public override int GetHashCode() {
    return FullPath().GetHashCode();
  }

  private string MapSegmentToString(ValuePathSegment s) {
    return s switch {
      ValuePathSegment.Raw r => r.Path,
      ValuePathSegment.ExpressionBased<object> eb => ConvertExpressionToPathSegment(eb.PathExpression),
      _ => throw new ArgumentOutOfRangeException(nameof(s))
    };
  }

  private string ConvertExpressionToPathSegment(Expression<Func<object, object>> pathExpression) {
    var memberExpression = pathExpression.Body is UnaryExpression expression
      ? (MemberExpression) expression.Operand
      : (MemberExpression) pathExpression.Body;

    return memberExpression.Member.Name;
  }
}

public abstract record ValuePathSegment {
  public sealed record Raw(string Path) : ValuePathSegment;

  public sealed record ExpressionBased<T>(Expression<Func<T, object>> PathExpression): ValuePathSegment;

}