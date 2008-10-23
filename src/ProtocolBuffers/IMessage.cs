// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
// http://github.com/jskeet/dotnet-protobufs/
// Original C++/Java/Python code:
// http://code.google.com/p/protobuf/
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above
// copyright notice, this list of conditions and the following disclaimer
// in the documentation and/or other materials provided with the
// distribution.
//     * Neither the name of Google Inc. nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
using System;
using System.Collections.Generic;
using System.IO;
using Google.ProtocolBuffers.Descriptors;

namespace Google.ProtocolBuffers {

  /// <summary>
  /// Non-generic interface used for all parts of the API which don't require
  /// any type knowledge.
  /// </summary>
  public interface IMessage {
    /// <summary>
    /// Returns the message's type's descriptor. This differs from the
    /// Descriptor property of each generated message class in that this
    /// method is an abstract method of IMessage whereas Descriptor is
    /// a static property of a specific class. They return the same thing.
    /// </summary>
    MessageDescriptor DescriptorForType { get; }
    /// <summary>
    /// Returns a collection of all the fields in this message which are set
    /// and their corresponding values.  A singular ("required" or "optional")
    /// field is set iff HasField() returns true for that field.  A "repeated"
    /// field is set iff GetRepeatedFieldSize() is greater than zero.  The
    /// values are exactly what would be returned by calling
    /// GetField(FieldDescriptor) for each field.  The map
    /// is guaranteed to be a sorted map, so iterating over it will return fields
    /// in order by field number. 
    /// </summary>
    IDictionary<FieldDescriptor, object> AllFields { get; }

    /// <summary>
    /// Returns true if the given field is set. This is exactly equivalent
    /// to calling the generated "Has" property corresponding to the field.
    /// </summary>
    /// <exception cref="ArgumentException">the field is a repeated field,
    /// or it's not a field of this type</exception>
    bool HasField(FieldDescriptor field);

    /// <summary>
    /// Obtains the value of the given field, or the default value if
    /// it isn't set. For value type fields, the boxed value is returned.
    /// For enum fields, the EnumValueDescriptor for the enum is returned.
    /// For embedded message fields, the sub-message
    /// is returned. For repeated fields, an IList&lt;T&gt; is returned.
    /// </summary>
    object this[FieldDescriptor field] { get; }

    /// <summary>
    /// Returns the number of elements of a repeated field. This is
    /// exactly equivalent to calling the generated "Count" property
    /// corresponding to the field.
    /// </summary>
    /// <exception cref="ArgumentException">the field is not a repeated field,
    /// or it's not a field of this type</exception>
    int GetRepeatedFieldCount(FieldDescriptor field);

    /// <summary>
    /// Gets an element of a repeated field. For value type fields 
    /// including enums, the boxed value is returned. For embedded
    /// message fields, the sub-message is returned.
    /// </summary>
    /// <exception cref="ArgumentException">the field is not a repeated field,
    /// or it's not a field of this type</exception>
    /// <exception cref="ArgumentOutOfRangeException">the index is out of
    /// range for the repeated field's value</exception>
    object this[FieldDescriptor field, int index] { get; }

    /// <summary>
    /// Returns the unknown fields for this message.
    /// </summary>
    UnknownFieldSet UnknownFields { get; }

    /// <summary>
    /// Returns true iff all required fields in the message and all embedded
    /// messages are set.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Serializes the message and writes it to the given output stream.
    /// This does not flush or close the stream.
    /// </summary>
    /// <param name="output"></param>
    void WriteTo(CodedOutputStream output);

    /// <summary>
    /// Returns the number of bytes required to encode this message.
    /// The result is only computed on the first call and memoized after that.
    /// </summary>
    int SerializedSize { get; }

    #region Comparison and hashing
    /// <summary>
    /// Compares the specified object with this message for equality.
    /// Returns true iff the given object is a message of the same type
    /// (as defined by DescriptorForType) and has identical values
    /// for all its fields.
    /// </summary>
    bool Equals(object other);

    /// <summary>
    /// Returns the hash code value for this message.
    /// TODO(jonskeet): Specify the hash algorithm, but better than the Java one!
    /// </summary>
    int GetHashCode();
    #endregion

    #region Convenience methods
    /// <summary>
    /// Converts the message to a string in protocol buffer text format.
    /// This is just a trivial wrapper around TextFormat.PrintToString.
    /// </summary>
    string ToString();

    /// <summary>
    /// Serializes the message to a ByteString. This is a trivial wrapper
    /// around WriteTo(CodedOutputStream).
    /// </summary>
    ByteString ToByteString();

    /// <summary>
    /// Serializes the message to a byte array. This is a trivial wrapper
    /// around WriteTo(CodedOutputStream).
    /// </summary>
    byte[] ToByteArray();

    /// <summary>
    /// Serializes the message and writes it to the given stream.
    /// This is just a wrapper around WriteTo(CodedOutputStream). This
    /// does not flush or close the stream.
    /// </summary>
    /// <param name="output"></param>
    void WriteTo(Stream output);
    #endregion

    /// <summary>
    /// Creates a builder for the type, but in a weakly typed manner. This
    /// is typically implemented by strongly typed builders by just returning
    /// the result of CreateBuilderForType.
    /// </summary>
    IBuilder WeakCreateBuilderForType();

    IMessage WeakDefaultInstanceForType { get; }
  }

  public interface IMessage<TMessage> : IMessage {
    /// <summary>
    /// Returns an instance of this message type with all fields set to
    /// their default values. This may or may not be a singleton. This differs
    /// from the DefaultInstance property of each generated message class in that this
    /// method is an abstract method of IMessage whereas DefaultInstance is
    /// a static property of a specific class. They return the same thing.
    /// </summary>
    TMessage DefaultInstanceForType { get; }
  }

  /// <summary>
  /// Type-safe interface for all generated messages to implement.
  /// </summary>
  public interface IMessage<TMessage, TBuilder> : IMessage<TMessage>
      where TMessage : IMessage<TMessage, TBuilder> 
      where TBuilder : IBuilder<TMessage, TBuilder> {
    #region Builders
    /// <summary>
    /// Constructs a new builder for a message of the same type as this message.
    /// </summary>
    TBuilder CreateBuilderForType();
    #endregion
  }
}
