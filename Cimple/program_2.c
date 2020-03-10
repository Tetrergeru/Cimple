u64 sum(u64 x, u64 y)
{
	(x += y);
	return x;
}

u64 main()
{
	u64 x;
	(x = sum(10, 20));
	printf(x);
	return 0;
}

