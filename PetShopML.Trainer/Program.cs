// See https://aka.ms/new-console-template for more information
using PetShopML.Trainer;

Console.WriteLine("Hello, Petshop Trainer");

var trainer = new PetShopTrainer();
trainer.Init();
var completed = trainer.Train();

if (completed) 
{
    trainer.Publish();
}


