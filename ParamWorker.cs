using System.Collections.Generic;

namespace Game.Web
{
    
    /// <summary>
    /// 封装/解封装参数的worker:参数类似与网页参数
    /// </summary>
    /// <remarks>例如参数"a=3&b=13&c=1080"</remarks>
	class ParamWorker
	{
        const char equalChar = '=';
        const char appendChar = '&';
        
        /// <summary>
        /// 将Dictionary打包为string参数形式
        /// </summary>
        public static string PackParams(Dictionary<string,string> dictParams)
        {
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            
            foreach (KeyValuePair<string, string> pair in dictParams)
            {
                strBuilder.Append(pair.Key );
                strBuilder.Append(equalChar);       //=
                strBuilder.Append(pair.Value);
                strBuilder.Append(appendChar);      //&
            }
            
            return strBuilder.ToString();
        }

        /// <summary>
        /// 将string参数解包为dictionary
        /// </summary>
		public static bool UnpackParams(string paramStr , out Dictionary<string, string>  result)
        {
			result= null;

            //打散pairs
            string[] pairsStrs = paramStr.Split(appendChar);
            int pairCount = paramStr.Length;

            //合法性：
            if (pairCount <= 0)
            {
                return false;
            }

			result = new Dictionary<string,string>();

            string[] keyValuePair = null;

            //遍历pair
            for (int i = 0; i < pairCount; i++)
            {
                keyValuePair = pairsStrs[i].Split(equalChar);
                //Check key value pair
                if (keyValuePair.Length != 2)
                {
                    continue;
                }
                else
                {
                    result.Add(keyValuePair[0], keyValuePair[1]);
                }
            }


            return true;
        }

	}
}
