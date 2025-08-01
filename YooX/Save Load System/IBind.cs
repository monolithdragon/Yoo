namespace YooX.SaveLoadSystem {
	public interface IBind<in TData> where TData : ISaveable {
		SerializableGuid Id { get; set; }
		void Bind(TData data);
	}
}