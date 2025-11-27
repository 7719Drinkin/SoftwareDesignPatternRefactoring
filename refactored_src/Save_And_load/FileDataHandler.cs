using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    private bool encryptData = false;
    private string codeWord = "cbx666";

    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
    }

    public void Save(GameData _data)
    {
        // 构建完整文件路径
        // 例如: "C:/Users/Player/AppData/LocalLow/MyGame/save.json"
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            // 创建目录结构
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // 序列化数据
            string dataToStore = JsonUtility.ToJson(_data, true);

            if (encryptData)
                dataToStore = EncryptDecrypt(dataToStore);

            // 创建新文件，如果文件已存在则覆盖
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("保存数据错误: " + fullPath + "\n" + ex);
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData data = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // 与同一个数进行两次异或运算，结果是原数字
                string primary = encryptData ? EncryptDecrypt(dataToLoad) : dataToLoad;

                // 首次尝试按当前加密配置解析
                try
                {
                    if (!string.IsNullOrWhiteSpace(primary))
                        data = JsonUtility.FromJson<GameData>(primary);
                }
                catch (System.Exception)
                {
                    data = null;
                }

                // 若失败，尝试反向解密（用于处理历史存档/配置不一致导致的读取失败）
                if (data == null)
                {
                    string fallback = encryptData ? dataToLoad : EncryptDecrypt(dataToLoad);
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(fallback))
                            data = JsonUtility.FromJson<GameData>(fallback);
                    }
                    catch (System.Exception)
                    {
                        data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("读取数据错误: " + fullPath + "\n" + ex);
            }
        }

        return data;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";

        for (int i = 0; i < data.Length; i++)
            modifiedData += (char)(data[i] ^ codeWord[i % codeWord.Length]);

        return modifiedData;
    }
}
