namespace OrleansBook.GrainInterfaces
{
  public class InstructionMessage
  {
    public InstructionMessage()
    { }

    public InstructionMessage(string instruction, string robot)
    {
      this.Instruction = instruction;
      this.Robot = robot;
    }

    public string Instruction { get; set; }
    public string Robot { get; set; }
  }

}
