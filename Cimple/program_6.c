#extern <printf>

u64 main()
{
	u64 s2;
	u64 s1;
	(s2 = 0x0021646c726f);
	(s1 = 0x77202c6f6c6c6548);
	printf((&s1));
	return 0;
}