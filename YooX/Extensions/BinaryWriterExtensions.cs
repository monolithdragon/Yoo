using System.IO;

namespace YooX {
	static public class BinaryWriterExtensions {
		static public void Write(this BinaryWriter writer, SerializableGuid guid) {
			writer.Write(guid.part1);
			writer.Write(guid.part2);
			writer.Write(guid.part3);
			writer.Write(guid.part4);
		}
	}
}