namespace LoggingConsole {
    public record LoggingPackage(int Level, string Type, string Message, long Timestamp) {

        public void Write(BinaryWriter writer) {
            writer.Write(Level);
            writer.Write(Type);
            writer.Write(Message);
            writer.Write(Timestamp);
            writer.Flush();
        }

        internal static LoggingPackage? Read(BinaryReader streamReader) {
            try {
                var level = streamReader.ReadInt32();
                var type = streamReader.ReadString();
                var message = streamReader.ReadString();
                var timestamp = streamReader.ReadInt64();

                return new(level, type, message, timestamp);
            } catch (EndOfStreamException) {
                return null;
            }
        }
    }
}
