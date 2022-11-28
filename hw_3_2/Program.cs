using System.Text;

interface IDataSource
{
	public string? Filename { get; set; }
	void WriteData(string data);
	string ReadData();
}


class FileDataSource : IDataSource
{
	public string? Filename { get; set; }
	public FileDataSource(string path)
		=> Filename = path;

	public void WriteData(string data)
	{
		using FileStream fs = new FileStream(Filename, FileMode.OpenOrCreate);
		using StreamWriter sw = new StreamWriter(fs);
		sw.Write(data);
	}

	public string ReadData()
	{
		using StreamReader sr = new StreamReader(Filename);
		string data = sr.ReadToEnd();
		return data;
	}
}

class Application
{
	private readonly IDataSource _datasource;

	public Application(IDataSource datasource)
		=> _datasource = datasource;

	public void WriteData(string data)
		=> _datasource.WriteData(data);

	public string ReadData(string path)
		=> _datasource.ReadData();
}

abstract class DataSourceDecorator : IDataSource
{
	public string? Filename { get; set; }
	protected IDataSource _wrappe { get; set; }

	public DataSourceDecorator(IDataSource datasource, string file)
	{
		_wrappe = datasource;
		Filename = file;
	}

	public virtual void WriteData(string message)
	{
		// Write Data
	}

	public virtual string ReadData()
	{
		// Read Data

		return "";
	}
}


class EncryptionDecorator : DataSourceDecorator
{
	char key = 'H';
	public EncryptionDecorator(string FileName, IDataSource? datasource = null)
		: base(datasource, FileName) { }

	public override string ReadData()
	{
		using StreamReader sr = new StreamReader(Filename);
		string data = sr.ReadToEnd();
		var decrypted = new StringBuilder();
		string sKey = "";

		for (int i = 0; i < data.Length; i++)
			sKey = sKey + char.ToString((char)(data[i] ^ key));

		return sKey.ToString();
	}

	public override void WriteData(string data)
	{
		int len = data.Length;
		string sKey = "";

		for (int i = 0; i < len; i++)
			sKey = sKey + char.ToString((char)(data[i] ^ key));

		using FileStream fs = new FileStream(Filename, FileMode.OpenOrCreate);
		using StreamWriter sw = new StreamWriter(fs);
		sw.Write(sKey);
	}
}

class CompressionDecorator : DataSourceDecorator
{
	public CompressionDecorator(IDataSource datasource, string FileName)
		: base(datasource, FileName) { }

	public override string ReadData()
	{
		// Read Data

		return "";
	}

	public override void WriteData(string message)
	{
		// Write Data
	}
}

class Program
{
	static void Main()
	{
		string FileName = "hsynnrn.txt";
		string path = @$"C:\Users\{Environment.UserName}\Desktop\{FileName}";

		IDataSource dataSource = new FileDataSource(path);
		dataSource = new EncryptionDecorator(path);
		dataSource.WriteData("We share the same spirit...");

		Console.WriteLine(dataSource.ReadData());
	}
}