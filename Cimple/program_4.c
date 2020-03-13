u64 mul(u64 x, u64 y)
{
	u64 result;
	(result = 0);
	while(((y - 1) >= 0))
	{ 
		(result += x);
		(y -= 1);
	}
	return result;
}

u64 main()
{
	u64 x;
	(x = 0);
	(x = ((2 == 3) != (3 == 2)));
	if (x)
	{
		printd(100);
	}
	else
	{
		printd(42);
	}
}