using System.Collections.Generic;

/// <summary>
/// 提供缓存机制的文件buffer，在超过一定长度之后自动删去之前的文件缓存，采取队列删除策略
/// </summary>
class FileBuffer
{
    //文件结构管理
    private class CFile
    {
        public string fileName;
        public string fileContent;
        public CFile(string fName, string fContent)
        {
            fileName = fName;
            fileContent = fContent;
        }
    }

    List<CFile> g_cfgFileBuffer = new List<CFile>();

    int m_maxBufferCount = 3;

    public void SetBufferSize(int count)
    {
        m_maxBufferCount = count;
    }

    public int GetBufferSize()
    {
        return m_maxBufferCount;
    }

    /// <summary>
    /// 将文件名、文件内容放入缓冲区
    /// </summary>
    public void PushToBuffer(string fileName, string fileContent)
    {
        if (g_cfgFileBuffer.Count + 1 > m_maxBufferCount)
        {
            //Pop the first one:
            g_cfgFileBuffer.RemoveAt(0);
        }

        g_cfgFileBuffer.Add(new CFile(fileName, fileContent));
    }

    /// <summary>
    /// 获得对应文件名的缓存，如果不存在则返回false
    /// </summary>
    public bool GetFileContent(string fileName, out string fileContent)
    {
        for (int i = 0; i < g_cfgFileBuffer.Count; i++)
        {
            if (g_cfgFileBuffer[i].fileName == fileName)
            {
                fileContent = g_cfgFileBuffer[i].fileContent;
                return true;
            }
        }

        fileContent = null;
        return false;
    }
}