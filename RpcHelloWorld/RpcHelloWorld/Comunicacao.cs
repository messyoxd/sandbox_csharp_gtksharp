// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: protos/comunicacao.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from protos/comunicacao.proto</summary>
public static partial class ComunicacaoReflection {

  #region Descriptor
  /// <summary>File descriptor for protos/comunicacao.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static ComunicacaoReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Chhwcm90b3MvY29tdW5pY2FjYW8ucHJvdG8iHAoEQ2FzYRIJCgF4GAEgASgF",
          "EgkKAXkYAiABKAUiFQoITWVuc2FnZW0SCQoBbRgDIAEoCTIyCglDb211bmlj",
          "YW8SJQoPU2VuZENvb3JkaW5hdGVzEgUuQ2FzYRoJLk1lbnNhZ2VtIgBiBnBy",
          "b3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::Casa), global::Casa.Parser, new[]{ "X", "Y" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::Mensagem), global::Mensagem.Parser, new[]{ "M" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class Casa : pb::IMessage<Casa> {
  private static readonly pb::MessageParser<Casa> _parser = new pb::MessageParser<Casa>(() => new Casa());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<Casa> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ComunicacaoReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Casa() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Casa(Casa other) : this() {
    x_ = other.x_;
    y_ = other.y_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Casa Clone() {
    return new Casa(this);
  }

  /// <summary>Field number for the "x" field.</summary>
  public const int XFieldNumber = 1;
  private int x_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int X {
    get { return x_; }
    set {
      x_ = value;
    }
  }

  /// <summary>Field number for the "y" field.</summary>
  public const int YFieldNumber = 2;
  private int y_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Y {
    get { return y_; }
    set {
      y_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as Casa);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(Casa other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (X != other.X) return false;
    if (Y != other.Y) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (X != 0) hash ^= X.GetHashCode();
    if (Y != 0) hash ^= Y.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (X != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(X);
    }
    if (Y != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(Y);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (X != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(X);
    }
    if (Y != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Y);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(Casa other) {
    if (other == null) {
      return;
    }
    if (other.X != 0) {
      X = other.X;
    }
    if (other.Y != 0) {
      Y = other.Y;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          X = input.ReadInt32();
          break;
        }
        case 16: {
          Y = input.ReadInt32();
          break;
        }
      }
    }
  }

}

public sealed partial class Mensagem : pb::IMessage<Mensagem> {
  private static readonly pb::MessageParser<Mensagem> _parser = new pb::MessageParser<Mensagem>(() => new Mensagem());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<Mensagem> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ComunicacaoReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Mensagem() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Mensagem(Mensagem other) : this() {
    m_ = other.m_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Mensagem Clone() {
    return new Mensagem(this);
  }

  /// <summary>Field number for the "m" field.</summary>
  public const int MFieldNumber = 3;
  private string m_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string M {
    get { return m_; }
    set {
      m_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as Mensagem);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(Mensagem other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (M != other.M) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (M.Length != 0) hash ^= M.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (M.Length != 0) {
      output.WriteRawTag(26);
      output.WriteString(M);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (M.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(M);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(Mensagem other) {
    if (other == null) {
      return;
    }
    if (other.M.Length != 0) {
      M = other.M;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 26: {
          M = input.ReadString();
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code
