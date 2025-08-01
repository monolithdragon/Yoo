using System.IO;

namespace YooX {
	public static class BinaryWriterExtensions {
		public static void Write(this BinaryWriter writer, SerializableGuid guid) {
			writer.Write(guid.part1);
			writer.Write(guid.part2);
			writer.Write(guid.part3);
			writer.Write(guid.part4);
		}
	}
}