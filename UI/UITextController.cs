using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[RequireComponent(typeof(Text))]
public class UITextController : MonoBehaviour {
    //原则：尽量不提供成员变量
    bool m_isPrinting =false;
    [SerializeField]bool m_isChinese = false;


    public void Reset( )
    {
        m_isPrinting = false;
    }

    /// <summary>
    /// 采用打印文字的方式将Text打印出来，可提供回调函数
    /// </summary>
    /// <param name="cbOnPrintOver">回调函数，无参数</param>
    public void StartPrintTextOneByOne ( CallbackFunc cbOnPrintOver )
    {
        if (m_isPrinting)
            return;

        m_isPrinting = true;
        Text printText = GetComponent<Text>( );
        if ( ( int )printText.text[0] < 128 )
        {
            //start with English
            m_isChinese = false;
        }
        else 
        {
            //Start with chinese
            m_isChinese = true;
        }

        if ( m_isChinese )
            printText.fontSize = 38;
        
        StartCoroutine( IPrintText( printText ,cbOnPrintOver) );
    }

    private IEnumerator IPrintText ( Text printText , CallbackFunc funC )
    {
        string src = printText.text;
        int strCount = src.Length;
        float waitSecs=  0.1F;
        float actualWaitTime = 0;

        float yUpDelta = Screen.height * 0.6F;
        int clickTimes = 0;
        Vector3 preDefaultPos = printText.rectTransform.position;           //原先的位置
        //i start from 1
        for ( int i = 1 ; i < strCount ; i++ )
        {
            printText.text = src.Substring( 0 , i + 1 );

            if ( m_isChinese || 
                (!m_isChinese&& src[i]!=' '))//英文状态下才对空格进行跳过等待，中文需要用之进行占位符等待
                actualWaitTime = waitSecs;
            
            

            if ( src[i] == '\n' )
            {
                actualWaitTime += waitSecs;
            }
            //Wait for it:
            while ( actualWaitTime >= 0 )
            {
                actualWaitTime -= Time.deltaTime;
                yield return 1;

                //Check key every frame:
                if ( Input.anyKeyDown )
                {
                    int timesCount = 4;
                    int addWords = 100;

                    if ( m_isChinese )
                    {
                        addWords = 60;
                    }

                    //every x times click to show next 100 chars
                    if ( clickTimes % timesCount == 0 )
                    {
                        i += addWords;
                        if ( i > strCount - 2 )
                            i = strCount - 2;
                    }
                    else
                    {
                        //Other click to lift up the view
                        printText.rectTransform.Translate( 0 , Screen.height * 0.135F , 0 );
                    }

                    clickTimes++;
                }

            }

            int moveCount = 128;
            int wordsCountToLiftUp=3;
            if ( m_isChinese )
            {
                moveCount = 30;
                wordsCountToLiftUp = 2;
            }
            
            if ( i > moveCount && ( i % wordsCountToLiftUp == 0 ) )
            {
                printText.rectTransform.Translate( 0 , yUpDelta * Time.deltaTime , 0 );
            }

        }
        //新注释的Debug.Log( "end of text. Try to call back?" + ( funC != null ) );
        m_isPrinting = false;

        

        if ( funC != null )
        {
            while ( true )
            {
                //Wait for click to continue:
                if ( Input.anyKeyDown )
                {
                    funC( );
                    printText.rectTransform.position = preDefaultPos;
                    break;
                }
                yield return 1;
            }
            
        }
    }
}
