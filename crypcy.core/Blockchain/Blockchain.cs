using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

public class Blockchain
{
    public string BlockchainName { get; set; }
    public IList<Block> Chain { set; get; }

    public Blockchain(string blockchainName)
    {
        BlockchainName = blockchainName;
        InitializeChain();
        AddGenesisBlock();
    }

    public void InitializeChain()
    {
        Chain = new ObservableCollection<Block>();
    }

    public Block CreateGenesisBlock()
    {
        return new Block("None", "Genesis Block");
    }

    public void AddGenesisBlock()
    {
        Chain.Add(CreateGenesisBlock());
    }


    public Block GetLatestBlock()
    {
        return Chain[Chain.Count - 1];
    }

    public void AddBlock(Block block)
    {
        Block latestBlock = GetLatestBlock();
        block.Index = latestBlock.Index + 1;
        block.PreviousHash = latestBlock.Hash;
        block.Hash = block.CalculateHash();
        Chain.Add(block);
    }


    public bool IsValid()
    {
        for (int i = 1; i < Chain.Count; i++)
        {
            Block currentBlock = Chain[i];
            Block previousBlock = Chain[i - 1];

            if (currentBlock.Hash != currentBlock.CalculateHash())
            {
                return false;
            }

            if (currentBlock.PreviousHash != previousBlock.Hash)
            {
                return false;
            }
        }
        return true;
    }
}

