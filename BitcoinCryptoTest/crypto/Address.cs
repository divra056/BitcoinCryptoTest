﻿using System;
using System.Security.Cryptography;
using System.Linq;

namespace Bitcoin_Tool.Crypto
{
	public class Address
	{
		const Byte PUBKEYHASH = 0x00;
		const Byte SCRIPTHASH = 0x05;
		const Byte PUBKEY = 0xFF;

		private String address = null;
		private Hash pubKeyHash = null;
		private Hash scriptHash = null;

		public Hash PubKeyHash
		{
			get
			{
				if (pubKeyHash == null && calcHash() != PUBKEYHASH)
					throw new InvalidOperationException("Address is not a public key hash.");
				return pubKeyHash;
			}
		}

		public Hash ScriptHash
		{
			get
			{
				if (pubKeyHash == null && calcHash() != SCRIPTHASH)
					throw new InvalidOperationException("Address is not a script hash.");
				return scriptHash;
			}
		}

		public Hash EitherHash
		{
			get
			{
				if (pubKeyHash == null && scriptHash == null)
					calcHash();
				if (pubKeyHash != null)
					return pubKeyHash;
				if (scriptHash != null)
					return scriptHash;
				return null;
			}
		}

		public Address(Byte[] data, Byte version)
		{
			SHA256 sha256 = new SHA256Managed();
			RIPEMD160 ripemd160 = new RIPEMD160Managed();
			switch (version)
			{
				case PUBKEY:
					pubKeyHash = ripemd160.ComputeHash(sha256.ComputeHash(data));
					break;
				case PUBKEYHASH:
					pubKeyHash = data;
					break;
				case SCRIPTHASH:
					scriptHash = data;
					break;
			}
		}

		public Address(String address)
		{
			this.address = address;
		}

		private Byte calcHash()
		{
			Byte version;
			Byte[] hash = Base58CheckString.ToByteArray(address, out version);
			switch (version)
			{
				case PUBKEYHASH:
					pubKeyHash = hash;
					break;
				case SCRIPTHASH:
					scriptHash = hash;
					break;
			}
			return version;
		}

		private void calcBase58()
		{
			if (pubKeyHash != null)
				this.address = Base58CheckString.FromByteArray(pubKeyHash, PUBKEYHASH);
			else
				throw new InvalidOperationException("Address is not a public key or script hash!");
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Address))
				return false;
			if (this.EitherHash == null || ((Address)obj).EitherHash == null)
				return false;
			return this.EitherHash.hash.SequenceEqual(((Address)obj).EitherHash.hash);
		}

		public override int GetHashCode()
		{
			return this.EitherHash.GetHashCode();
		}

		public override String ToString()
		{
			if (address == null)
				calcBase58();
			return address;
		}
	}
}
