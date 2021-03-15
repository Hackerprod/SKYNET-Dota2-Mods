namespace SevenZip
{
	internal interface ICodeProgress
	{
		void SetProgress(long inSize, long outSize);
	}
}
