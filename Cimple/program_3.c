#extern <printf>

u64 print_number(u64 x)
{
	u64 string;
	(string = 0x000a756c25);
	return printf((&string), x);
}

u64 main()
{
	u64 char;
	u64 x;
	(x = 0x0020726f727265);
	(char = ((x >> 8) & 0xff));
	printf((&char));
}