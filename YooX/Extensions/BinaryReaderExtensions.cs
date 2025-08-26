using System.IO;

namespace YooX {
	static public class BinaryReaderExtensions {
		static public SerializableGuid Read(this BinaryReader reader) => new(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
	}
}