using System;

namespace YooX {
	static public class GuidExtensions {
		static public SerializableGuid ToSerializableGuid(this Guid systemGuid) {
			byte[] bytes = systemGuid.ToByteArray();

			return new SerializableGuid(
				BitConverter.ToUInt32(bytes, 0),
				BitConverter.ToUInt32(bytes, 4),
				BitConverter.ToUInt32(bytes, 8),
				BitConverter.ToUInt32(bytes, 12)
			);
		}

		static public Guid ToSystemGuid(this SerializableGuid serializableGuid) {
			var bytes = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.part1), 0, bytes, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.part2), 0, bytes, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.part3), 0, bytes, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(serializableGuid.part4), 0, bytes, 12, 4);

			return new Guid(bytes);
		}
	}
}