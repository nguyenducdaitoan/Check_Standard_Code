using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace CheckSTP
{
    public partial class Form1 : Form
    {
        private const string FILEDATAPATH = "RuleData.xml";
        private const string FILECODEPATH = "RuleCode.xml";

        public Form1()
        {
            CultureInfo culEnglish = new CultureInfo( "vi-VN" );
            InitializeComponent();
            Application.CurrentCulture = culEnglish;
            this.Refresh();
        }

        private void btnCheck_Click( object sender, EventArgs e )
        {
            if ( txtSQLPath.Text.Length == 0 )
            {
                MessageBox.Show( "Please choose SQL Folder Location" );
                return;
            }
            //Prepare RegEx
            string regExUnUsedVariables = "([@][a-zA-z0-9_]+)([ ,\t]*)";
            Regex objUnusedVariablePattern = new Regex( regExUnUsedVariables );
            Hashtable hstUnsueVar = new Hashtable();

            //Get content file
            string sqlContent = string.Empty;
            string strSqlUnuseVariable = string.Empty;
            StringBuilder sb = new StringBuilder();

            DirectoryInfo stpDir = new DirectoryInfo( txtSQLPath.Text );
            FileInfo[] sqlFiles = stpDir.GetFiles( "*.sql", SearchOption.AllDirectories );
            foreach ( FileInfo sqlFile in sqlFiles )
            {
                //Read file content
                sqlContent = File.ReadAllText( sqlFile.FullName );//, Encoding.GetEncoding( "shift_jis" ) );

                //Search variable name
                MatchCollection matches = objUnusedVariablePattern.Matches( sqlContent );

                //Prepare output format
                //sb.AppendFormat( "\r\nChecking {0} in {1}:", sqlFile.Name, sqlFile.Directory.Name );
                sb.AppendFormat( "\r\n{1}\t{0}:", sqlFile.Name, sqlFile.Directory.Name );
                hstUnsueVar.Clear();

                //Check Unused variables
                foreach ( Match match in matches )
                {
                    //sb.AppendFormat( "{0}\r\n", match.Groups[1].Value );
                    if ( hstUnsueVar.ContainsKey( match.Groups[ 1 ].Value ) )
                    {
                        // If duplicate item then remove them
                        //hstUnsueVar.Remove( match.Value.Trim() );
                        hstUnsueVar[ match.Groups[ 1 ].Value ] = 1;
                    }
                    else
                    {
                        sqlContent = match.Groups[ 1 ].Value.ToLower();
                        if ( !sqlContent.Contains( "rowcount" )
                            && !sqlContent.Contains( "controldate" )
                            && !sqlContent.Contains( "ct_onflg" )
                            && !sqlContent.Contains( "ct_offflg" )
                            && !sqlContent.Contains( "fetch_status" )
                            && !sqlContent.Contains( "fetch_status" )
                            )
                        {
                            hstUnsueVar.Add( match.Groups[ 1 ].Value, 0 );
                        }
                    }
                }
                //Collect result
                bool hasData = false;
                foreach ( DictionaryEntry unUseVar in hstUnsueVar )
                {
                    int val = (int)unUseVar.Value;

                    if ( val == 0 )
                    {
                        hasData = true;
                        sb.AppendFormat( "\r\n\t\t{0}", unUseVar.Key );
                    }
                }
                if ( !hasData )
                {
                    sb.Append( "\tOK\r\n" );
                }
            }

            //Output Result
            rtbSQLUnusedVariable.Text = sb.ToString();

        }

        private void btnChooseSQLFolder_Click( object sender, EventArgs e )
        {
            using ( FolderBrowserDialog folderDlg = new FolderBrowserDialog() )
            {
                folderDlg.SelectedPath = txtSQLPath.Text;
                folderDlg.ShowDialog();
                if ( folderDlg.SelectedPath.Length > 0 )
                {
                    txtSQLPath.Text = folderDlg.SelectedPath;
                }
            }
        }

        private void CheckSTD_Click( object sender, EventArgs e )
        {
            if ( txtSQLPath.Text.Length == 0 )
            {
                MessageBox.Show( "Please choose SQL Folder Location" );
                return;
            }

            rtbSQLUnusedVariable.Clear();

            //Prepare RegEx
            string pattern = string.Empty;
            Regex objRegEx = null;

            DirectoryInfo stpDir = new DirectoryInfo( txtSQLPath.Text );
            FileInfo[] sqlFiles = stpDir.GetFiles( "*.sql", SearchOption.AllDirectories );
            ruleSTDDs.Clear();
            ruleSTDDs.ReadXml( FILEDATAPATH );

            //Get content file
            string[] sqlLines = null;
            string report = string.Empty;
            int countLine = 0;
            string jobName = string.Empty;
            string jobOldName = string.Empty;

            int countDir = 0;
            bool isMatch = false;

            foreach ( FileInfo sqlFile in sqlFiles )
            {
                bool hasErr = false;
                int countLineContainQueryWord = 0;
                int countLineContainENDCATCH = 0;
                int countComment = 0;  // count /* */ comment
                ArrayList indentContainer = new ArrayList();

                //Get jobName
                jobName = sqlFile.Directory.Name;
                if ( jobName != jobOldName )
                {
                    jobOldName = jobName;
                    rtbSQLUnusedVariable.SelectionFont = new Font( "Tahoma", 10, FontStyle.Bold );
                    rtbSQLUnusedVariable.SelectedText = string.Format( "\r\n{0}:", jobName );
                    countDir++;
                }
                rtbSQLUnusedVariable.SelectionFont = new Font( "Tahoma", 10, FontStyle.Bold );
                rtbSQLUnusedVariable.SelectedText = string.Format( "\r\n\t{0}:", sqlFile.Name );

                using ( var reader = new FileStream( sqlFile.FullName, FileMode.Open ) )
                {
                    bool isShiftJis = true;

                    byte[] buffer = new byte[ 3 ];
                    reader.Read( buffer, 0, 3 );

                    if ( buffer[ 0 ] == 0xef && buffer[ 1 ] == 0xbb && buffer[ 2 ] == 0xbf )
                        isShiftJis = false; // Encoding.UTF8;
                    else if ( buffer[ 0 ] == 0xfe && buffer[ 1 ] == 0xff )
                        isShiftJis = false; // Encoding.Unicode;
                    else if ( buffer[ 0 ] == 0 && buffer[ 1 ] == 0 && buffer[ 2 ] == 0xfe && buffer[ 3 ] == 0xff )
                        isShiftJis = false; // Encoding.UTF32;
                    else if ( buffer[ 0 ] == 0x2b && buffer[ 1 ] == 0x2f && buffer[ 2 ] == 0x76 )
                        isShiftJis = false; // Encoding.UTF7;
                    else if ( buffer[ 0 ] == 0xFE && buffer[ 1 ] == 0xFF )
                        // 1201 unicodeFFFE Unicode (Big-Endian)
                        isShiftJis = false; // Encoding.GetEncoding(1201);
                    else if ( buffer[ 0 ] == 0xFF && buffer[ 1 ] == 0xFE )
                        // 1200 utf-16 Unicode
                        isShiftJis = false; // Encoding.GetEncoding(1200);

                    if ( isShiftJis == false ) // khong phai encoding shift-jis
                    {
                        rtbSQLUnusedVariable.SelectionFont = new Font( "Tahoma", 10, FontStyle.Regular );
                        rtbSQLUnusedVariable.SelectedText = string.Format( "\r\n\t\t* {0}.", "File chưa được lưu với encoding SHIFT-JIS" );
                    }
                    else
                    {
                        rtbSQLUnusedVariable.SelectionFont = new Font( "Tahoma", 10, FontStyle.Regular );
                        rtbSQLUnusedVariable.SelectedText = string.Format( "\r\n\t\t* {0}.", "Hãy kiểm tra file được lưu với encoding SHIFT-JIS chưa" );
                    }
                }

                //Read file content
                sqlLines = null;
                sqlLines = File.ReadAllLines( sqlFile.FullName, Encoding.GetEncoding( "shift-jis" ) );

                foreach ( RuleSTD.RuleSTDRow row in ruleSTDDs._RuleSTD.Rows )
                {
                    pattern = row.Value;
                    objRegEx = new Regex( pattern );

                    foreach ( string sqlLine in sqlLines )
                    {
                        countLine++;

                        //Search variable name
                        Match match = objRegEx.Match( sqlLine + "\r\n" );
                        if ( countLine > 3 )	// Error
                        {
                            String sqlLineExceptCommentLine = string.Empty; // Chua string de loai tru dau ' asdfasdf' va dau comment --
                            String sqlLineExceptCommentSlashLine = string.Empty;
                            String sqlLineExcepBothCommentLineAndSingleQuote = string.Empty;

                            /*----------------------------------Loai tru dau '' va comment cua line----------------------------------------*/
                            int positionOfCommentSignal = sqlLine.IndexOf( "--" );
                            if ( positionOfCommentSignal == -1 )
                                positionOfCommentSignal = sqlLine.Length;

                            int positionOfCommentSlashSignal = sqlLine.IndexOf( "//" );
                            if ( positionOfCommentSlashSignal == -1 )
                                positionOfCommentSlashSignal = sqlLine.Length;

                            // sqlLineExceptCommentLine chua dong khong co comment
                            sqlLineExceptCommentLine = sqlLine.Substring( 0, positionOfCommentSignal );
                            sqlLineExceptCommentSlashLine = sqlLine.Substring( 0, positionOfCommentSlashSignal );

                            int firstPosOfSingleQuote = sqlLineExceptCommentLine.IndexOf( "'" );
                            int lastPosOfSingleQuote = sqlLineExceptCommentLine.LastIndexOf( "'" );

                            if ( firstPosOfSingleQuote > 0 && lastPosOfSingleQuote > 0 )
                                sqlLineExcepBothCommentLineAndSingleQuote = sqlLineExceptCommentLine.Substring( 0, firstPosOfSingleQuote - 1 ) + sqlLineExceptCommentLine.Substring( lastPosOfSingleQuote + 1 );
                            else
                                sqlLineExcepBothCommentLineAndSingleQuote = sqlLineExceptCommentLine;
                            /*----------------------------------Ket thuc loai tru-----------------------------------------------------------*/

                            if ( Regex.IsMatch( sqlLineExceptCommentLine, @"^([ \t]+)?\b(?:SELECT|INSERT|UPDATE|TRUNCATE\b)" ) ) // Tim vi tri xuat hien ky tu query dau tien (SELECT, INSERT, UPDATE, TRUNCATE)
                            {
                                if ( countLineContainQueryWord == 0 ) countLineContainQueryWord = countLine;
                            }
                            if ( Regex.IsMatch( sqlLineExceptCommentLine, @"END[ \t]+CATCH" ) ) // Tim vi tri xuat hien ky tu query dau tien (SELECT, INSERT, UPDATE, TRUNCATE)
                            {
                                if ( countLineContainENDCATCH == 0 )
                                    countLineContainENDCATCH = countLine;
                            }

                            // Loai tru cac rule check sau dau comment va dau ''
                            if ( row.Key == "String5" )
                            {
                                if ( objRegEx.IsMatch( sqlLineExcepBothCommentLineAndSingleQuote ) )
                                {
                                    report += string.Format( "\r\n\t\t\t + Line {0}:\t{1}", countLine.ToString(), sqlLineExceptCommentLine.Replace( '\u0009'.ToString(), " " ) );
                                    isMatch = true;
                                    hasErr = true;
                                }
                            }
                            else if ( row.Key == "String6" || row.Key == "String7" || row.Key == "String12" )
                            {
                                if ( objRegEx.IsMatch( sqlLineExcepBothCommentLineAndSingleQuote ) )
                                {
                                    report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                    isMatch = true;
                                    hasErr = true;
                                }
                            }// Ket thuc loai tru cho cac rule
                            else if ( row.Key == "String15" )
                            {
                                if ( countLine > countLineContainQueryWord ) // Tim vi tri xuat hien cua chuoi hang so trong dau ''
                                {
                                    if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}:\t{1}", countLine.ToString(), sqlLineExceptCommentLine.Replace( '\u0009'.ToString(), " " ) );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                            }
                            else if ( row.Key == "String23" )
                            {
                                if ( Regex.IsMatch( sqlLineExceptCommentLine, @"\bFW_STP_DisposeCursor\b" )
                                        || Regex.IsMatch( sqlLineExceptCommentLine, @"\bFW_STP_WriteLog\b" )
                                        || Regex.IsMatch( sqlLineExceptCommentLine, @"\bFW_STP_WriteErrorLog\b" ) ) // Tim vi tri xuat hien cua cac ham FW_STP_DisposeCursor, FW_STP_WriteLog, FW_STP_WriteErrLog
                                {
                                    if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                            }
                            else if ( row.Key == "String30" )
                            {
                                if ( sqlLineExceptCommentLine.Equals( "BEGIN" ) )
                                {
                                    if ( !objRegEx.IsMatch( sqlLines[ countLine ] ) ) // phu dinh IsMatch
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}", ( countLine + 1 ).ToString() );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                            }
                            else if ( row.Key == "String31" )
                            {
                                if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) ) // Xuat hien EXECUTE
                                {
                                    if ( !sqlLineExceptCommentLine.Contains( "sp_executesql" ) && !sqlLineExceptCommentLine.Contains( "dbo." ) ) // khong phai goi thuc thi cau sql thi phai co dbo. truoc ten STP duoc goi
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                            }
                            else if ( row.Key == "String32" ) // Indent IF & WHILE
                            {
                                if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) ) // Xuat hien IF or WHILE
                                {
                                    string spaceOrTabBeforeIFWHILE = "";
                                    string spaceOrTabBeforeBEGIN = "";
                                    try
                                    {
                                        if ( sqlLineExceptCommentLine.Contains( "IF" ) )
                                            spaceOrTabBeforeIFWHILE = sqlLineExceptCommentLine.Substring( 0, sqlLineExceptCommentLine.IndexOf( sqlLineExceptCommentLine.Trim() ) );
                                        else if ( sqlLineExceptCommentLine.Contains( "WHILE" ) )
                                            spaceOrTabBeforeIFWHILE = sqlLineExceptCommentLine.Substring( 0, sqlLineExceptCommentLine.IndexOf( sqlLineExceptCommentLine.Trim() ) );

                                        if ( sqlLines[ countLine ].Contains( "BEGIN" ) )
                                            spaceOrTabBeforeBEGIN = sqlLines[ countLine ].Substring( 0, sqlLines[ countLine ].IndexOf( "BEGIN" ) );
                                        else
                                            spaceOrTabBeforeBEGIN = sqlLines[ countLine ].Substring( 0, sqlLines[ countLine ].IndexOf( sqlLines[ countLine ].Trim() ) );

                                        if ( !spaceOrTabBeforeBEGIN.Equals( spaceOrTabBeforeIFWHILE ) )
                                        {
                                            report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                            isMatch = true;
                                            hasErr = true;
                                        }
                                    }
                                    catch ( Exception )
                                    {

                                    }
                                }
                            }
                            else if ( row.Key == "String34" )
                            {
                                if ( !objRegEx.IsMatch( sqlLineExceptCommentSlashLine ) )
                                {
                                    if ( sqlLineExceptCommentSlashLine.Contains( "usp_" ) )
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                            }
                            else if ( row.Key == "String35" )
                            {
                                if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                {
                                    countComment += 1;
                                    if ( countComment > 1 )
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                            }
                            else if ( row.Key == "String36" )
                            {
                                if ( sqlLineExceptCommentLine.Length == 0 || sqlLineExceptCommentLine == "" )
                                {
                                    if ( sqlLines[ countLine ].Length == 0 || sqlLines[ countLine ] == "" )
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                            }
                            else if ( row.Key == "String37" )
                            {
                                if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                {
                                    if ( sqlLines[ countLine ].Length == 0 || sqlLines[ countLine ] == "" )
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                            }
                            else if ( row.Key == "String38" )
                            {
                                if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                {
                                    if ( sqlLines[ countLine - 2 ].Length == 0 || sqlLines[ countLine - 2 ] == "" )
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                            }
                            /*
                            else if (row.Key == "String40")
                            {
                                if (sqlLineExceptCommentLine.Contains("BEGIN"))
                                {
                                    isInBlock = true;
                                    indentContainer.Add(sqlLineExceptCommentLine.Substring(0, sqlLineExceptCommentLine.IndexOf("BEGIN")));
                                }
                                else
                                {
                                    if (countLine == sqlLines.Length || indentContainer.Count == 0)
                                    {
                                        isInBlock = false;
                                    }

                                    if (sqlLineExceptCommentLine.Contains("END"))
                                    {
                                        if (indentContainer.Count > 0)
                                            indentContainer.RemoveAt(indentContainer.Count - 1);
                                    }

                                    if (sqlLineExceptCommentLine.Contains("COMMIT") && isCommitTran == false)
                                    {
                                        isCommitTran = true;

                                        if (indentContainer.Count > 0)
                                            indentContainer.RemoveAt(indentContainer.Count - 1);
                                    }

                                    if (isInBlock == true)  // dang o trong block BEGIN END
                                    {
                                        if (sqlLineExceptCommentLine.Trim() != "" && !sqlLineExceptCommentLine.Contains("END"))
                                        {
                                            // Get indent string
                                            indentStr = indentContainer[indentContainer.Count - 1].ToString();

                                            indentStrOfCurrentLine = sqlLine.Substring(0, sqlLine.IndexOf(sqlLine.Trim()));

                                            if (indentStr.Length + 1 != indentStrOfCurrentLine.Length)
                                            {
                                                if (!sqlLines[countLine - 2].Contains("DECLARE"))
                                                {
                                                    report += string.Format("\r\n\t\t\t + Line {0}", countLine.ToString());
                                                    isMatch = true;
                                                    hasErr = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            */
                            else if ( objRegEx.IsMatch( sqlLine ) )
                            {
                                report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                isMatch = true;
                                hasErr = true;
                            }
                        }
                    }
                    if ( isMatch ) // Cho ca file
                    {
                        rtbSQLUnusedVariable.SelectionFont = new Font( "Tahoma", 10, FontStyle.Regular );
                        rtbSQLUnusedVariable.SelectedText = string.Format( "\r\n\t\t* {0}:", row.Comment );
                        rtbSQLUnusedVariable.SelectedText = report;
                    }
                    report = string.Empty;
                    countLine = 0;
                    isMatch = false;
                }
                if ( hasErr == false )
                {
                    rtbSQLUnusedVariable.SelectedText = string.Format( "\t{0}\r\n", "OK" );
                }
            }
        }

        private void btnCheckCode_Click( object sender, EventArgs e )
        {
            try
            {
                if ( txtCodePath.Text.Length == 0 )
                {
                    MessageBox.Show( "Please choose Code Folder Location" );
                    return;
                }

                rtbSQLUnusedVariable.Clear();

                //Prepare RegEx
                string pattern = string.Empty;
                Regex objRegEx = null;

                DirectoryInfo cdeDir = new DirectoryInfo( txtCodePath.Text );
                FileInfo[] sqlFiles = null;

                //string lookFor = "*Jp.cs;*Ja.cs;*Exe.cs";
                string lookFor = "*.cs";
                string[] extensions = lookFor.Split( new char[] { ';' } );
                ArrayList myfileinfos = new ArrayList();

                foreach ( string ext in extensions )
                {
                    myfileinfos.AddRange( cdeDir.GetFiles( ext, SearchOption.AllDirectories ) );
                }

                sqlFiles = (FileInfo[])myfileinfos.ToArray( typeof( FileInfo ) );
                //= stpDir.GetFiles( "*.sql", SearchOption.AllDirectories );

                ruleSTDDs.Clear();
                ruleSTDDs.ReadXml( FILECODEPATH );

                //Get content file
                string[] sqlLines = null;
                string report = string.Empty;
                int countLine = 0;
                string jobName = string.Empty;
                string jobOldName = string.Empty;

                int countDir = 0;
                bool isMatch = false;

                foreach ( FileInfo sqlFile in sqlFiles )
                {
                    bool hasErr = false;
                    bool first3SlashComment = false;
                    bool first2SlashComment = false;
                    int beginCheckRegion = 0;

                    //Get jobName
                    jobName = sqlFile.Directory.Name;
                    if ( jobName != jobOldName )
                    {
                        jobOldName = jobName;
                        rtbSQLUnusedVariable.SelectionFont = new Font( "Tahoma", 10, FontStyle.Bold );
                        rtbSQLUnusedVariable.SelectedText = string.Format( "\r\n{0}:", jobName );
                        countDir++;
                    }
                    rtbSQLUnusedVariable.SelectionFont = new Font( "Tahoma", 10, FontStyle.Bold );
                    rtbSQLUnusedVariable.SelectedText = string.Format( "\r\n\t{0}:", sqlFile.Name );

                    //Read file content
                    sqlLines = File.ReadAllLines( sqlFile.FullName, Encoding.GetEncoding( "shift-jis" ) );
                    foreach ( RuleSTD.RuleSTDRow row in ruleSTDDs._RuleSTD.Rows )
                    {
                        pattern = row.Value;
                        objRegEx = new Regex( pattern );

                        foreach ( string sqlLine in sqlLines )
                        {
                            countLine++;
                            
                            //Search variable name
                            Match match = objRegEx.Match( sqlLine + "\r\n" );
                            if ( countLine > 0 )	// Error
                            {
                                String sqlLineExceptCommentLine = string.Empty; // Chua string de loai tru dau ' asdfasdf' va dau comment --
                                String sqlLineExcepBothCommentLineAndSingleQuote = string.Empty;

                                int positionOfRegion = sqlLine.IndexOf( @"#region" );

                                int positionOfCommentSignal = sqlLine.IndexOf( @"//" );
                                if ( positionOfCommentSignal == -1 )
                                    positionOfCommentSignal = sqlLine.Length;

                                sqlLineExceptCommentLine = sqlLine.Substring( 0, positionOfCommentSignal );

                                int firstPosOfSingleQuote = sqlLineExceptCommentLine.IndexOf( "'" );
                                int lastPosOfSingleQuote = sqlLineExceptCommentLine.LastIndexOf( "'" );

                                if ( firstPosOfSingleQuote > 0 && lastPosOfSingleQuote > 0 )
                                    sqlLineExcepBothCommentLineAndSingleQuote = sqlLineExceptCommentLine.Substring( 0, firstPosOfSingleQuote - 1 ) + sqlLineExceptCommentLine.Substring( lastPosOfSingleQuote + 1 );
                                else
                                    sqlLineExcepBothCommentLineAndSingleQuote = sqlLineExceptCommentLine;

                                if ( sqlLine.Contains( "using" ) )
                                    beginCheckRegion = countLine;

                                // Loai tru cac rule check sau dau comment va dau ''
                                if ( row.Key == "String5" || row.Key == "String6" )
                                {
                                    if ( positionOfRegion >= 0 )
                                        continue;
                                    if ( objRegEx.IsMatch( sqlLineExcepBothCommentLineAndSingleQuote ) )
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                                else if ( row.Key == "String12" )
                                {
                                    if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}:\t{1}", countLine.ToString(), sqlLineExceptCommentLine.Replace( '\u0009'.ToString(), " " ) );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                                else if ( row.Key == "String15" )
                                {
                                    if ( sqlLineExceptCommentLine.Length == 0 || sqlLineExceptCommentLine == "" )
                                    {
                                        if ( countLine < sqlLines.Length )
                                        {
                                            if ( sqlLines[ countLine ].Length == 0 || sqlLines[ countLine ] == "" )
                                            {
                                                report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                                isMatch = true;
                                                hasErr = true;
                                            }
                                        }
                                    }
                                }
                                else if ( row.Key == "String16" )
                                {
                                    if ( sqlLineExceptCommentLine.Trim() == "}" )
                                    {
                                        if ( sqlLines[ countLine - 2 ].Length == 0 || sqlLines[ countLine - 2 ] == "" )
                                        {
                                            if ( sqlLines[ countLine - 3 ].Trim() != "{" )
                                            {
                                                report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                                isMatch = true;
                                                hasErr = true;
                                            }
                                        }
                                    }
                                }
                                else if ( row.Key == "String17" )
                                {
                                    if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                    {
                                        if ( sqlLines[ countLine - 2 ].Length != 0 || sqlLines[ countLine - 2 ] != "" )
                                        {
                                            report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                            isMatch = true;
                                            hasErr = true;
                                        }
                                    }
                                }
                                else if ( row.Key == "String18" )
                                {
                                    if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) && !sqlLineExceptCommentLine.Contains( "}" ) )
                                    {
                                        if ( sqlLines[ countLine ].Length == 0 || sqlLines[ countLine ] == "" )
                                        {
                                            if ( sqlLines[ countLine + 1 ].Trim() != "}" )
                                            {
                                                report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                                isMatch = true;
                                                hasErr = true;
                                            }
                                        }
                                    }
                                }
                                else if ( row.Key == "String19" )
                                {
                                    if ( beginCheckRegion < countLine )  // Chi bat dau check tu khi co khai bao using (bo qua phan header)
                                    {
                                        if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                        {
                                            if ( sqlLines[ countLine ].Length != 0 || sqlLines[ countLine ] != "" )
                                            {
                                                report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                                isMatch = true;
                                                hasErr = true;
                                            }
                                        }
                                    }
                                }
                                else if ( row.Key == "String20" )
                                {
                                    if ( beginCheckRegion < countLine ) // Chi bat dau check tu khi co khai bao using (bo qua phan header)
                                    {
                                        if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                        {
                                            if ( sqlLines[ countLine - 2 ].Length != 0 || sqlLines[ countLine - 2 ] != "" )
                                            {
                                                report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                                isMatch = true;
                                                hasErr = true;
                                            }
                                        }
                                    }
                                }
                                else if ( row.Key == "String21" )
                                {
                                    if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                    {
                                        report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                        isMatch = true;
                                        hasErr = true;
                                    }
                                }
                                else if ( row.Key == "String22" )
                                {
                                    if ( objRegEx.IsMatch( sqlLine ) && first3SlashComment == false )
                                    {
                                        first3SlashComment = true;

                                        if ( sqlLines[ countLine - 2 ].Trim() != "{" && sqlLines[ countLine - 2 ].Trim() != "" )
                                        {
                                            report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                            isMatch = true;
                                            hasErr = true;
                                        }
                                    }

                                    if ( !objRegEx.IsMatch( sqlLine ) )
                                        first3SlashComment = false;
                                }
                                else if ( row.Key == "String24" )
                                {
                                    if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                    {
                                        if ( sqlLines[ countLine - 2 ].Trim() != "{" && sqlLines[ countLine - 2 ].Trim() != "" && !sqlLines[ countLine - 2 ].Trim().Contains( "//" ) )
                                        {
                                            report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                            isMatch = true;
                                            hasErr = true;
                                        }
                                    }
                                }
                                else if ( row.Key == "String25" )
                                {
                                    if ( countLine > beginCheckRegion )
                                    {
                                        if ( objRegEx.IsMatch( sqlLine ) && first2SlashComment == false )
                                        {
                                            first2SlashComment = true;

                                            if ( sqlLine.Trim().Substring( 0, 2 ).Equals( "//" ) )
                                            {
                                                if ( sqlLines[ countLine - 2 ].Trim() != "{" && sqlLines[ countLine - 2 ].Trim() != "" )
                                                {
                                                    report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                                    isMatch = true;
                                                    hasErr = true;
                                                }
                                            }
                                        }

                                        if ( !objRegEx.IsMatch( sqlLine ) )
                                            first2SlashComment = false;
                                    }
                                }
                                else if ( row.Key == "String29" )
                                {
                                    if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                    {
                                        if ( !sqlLineExceptCommentLine.Contains( "const" ) && !sqlLineExceptCommentLine.Contains( "#region" ) )
                                        {
                                            report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                            isMatch = true;
                                            hasErr = true;
                                        }
                                    }
                                }
                                else if ( row.Key == "String30" )
                                {
                                    if ( beginCheckRegion < countLine )  // Chi bat dau check tu khi co khai bao using (bo qua phan header)
                                    {
                                        if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                        {
                                            if ( sqlLines[ countLine ].Trim() != "" && sqlLines[ countLine ].Trim() != "}")
                                            {
                                                report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                                isMatch = true;
                                                hasErr = true;
                                            }
                                        }
                                    }
                                }
                                else if ( row.Key == "String31" )
                                {
                                    if ( beginCheckRegion < countLine ) // Chi bat dau check tu khi co khai bao using (bo qua phan header)
                                    {
                                        if ( objRegEx.IsMatch( sqlLineExceptCommentLine ) )
                                        {
                                            if ( sqlLines[ countLine - 2 ].Trim()!= "" && sqlLines[ countLine - 2 ].Trim() != "{" )
                                            {
                                                report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                                isMatch = true;
                                                hasErr = true;
                                            }
                                        }
                                    }
                                }
                                // Ket thuc loai tru cho cac rule
                                else if ( objRegEx.IsMatch( sqlLine ) )
                                {
                                    report += string.Format( "\r\n\t\t\t + Line {0}", countLine.ToString() );
                                    isMatch = true;
                                    hasErr = true;
                                }
                            }
                        }
                        if ( isMatch ) // Cho ca file
                        {
                            rtbSQLUnusedVariable.SelectionFont = new Font( "Tahoma", 10, FontStyle.Regular );
                            rtbSQLUnusedVariable.SelectedText = string.Format( "\r\n\t\t* {0}:", row.Comment );
                            rtbSQLUnusedVariable.SelectedText = report;
                        }
                        report = string.Empty;
                        countLine = 0;
                        isMatch = false;
                    }
                    if ( hasErr == false )
                    {
                        rtbSQLUnusedVariable.SelectedText = string.Format( "\t{0}\r\n", "OK" );
                    }
                }
            }
            catch ( Exception abc )
            {
                MessageBox.Show( "Lỗi rồi, vui lòng liên hệ Vinh đại ka để biết thêm chi tiết :(" );
                MessageBox.Show( abc.ToString() );
            }
        }

        private void button1_Click( object sender, EventArgs e )
        {
            using ( FolderBrowserDialog folderDlg = new FolderBrowserDialog() )
            {
                folderDlg.SelectedPath = txtCodePath.Text;
                folderDlg.ShowDialog();
                if ( folderDlg.SelectedPath.Length > 0 )
                {
                    txtCodePath.Text = folderDlg.SelectedPath;
                }
            }
        }
    }
}