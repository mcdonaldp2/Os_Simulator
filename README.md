# Os_Simulator #

An operating system simulator built on top of the Universal Windows Platform (UWP) created by Paul McDonald and Matthew McGee.

# Screenshots #

![](/relative/path/to/img.jpg?raw=true "Optional Title")
![simulator_screenshot.PNG](/Os_simulator/simulator_screenshot.png)

![simulator_graph_open.PNG](https://bitbucket.org/repo/5bqXoo/images/1821975699-simulator_graph_open.PNG)

# Commands #

**PROC** 

* shows all unfinished processes in the system and their information. The process information should include: current process state, amount of CPU time needed to complete, amount of CPU time already used, priority (if relevant), number of I/O requests performed. 

**MEM** 

* shows the current usage of memory space. 

**EXE** 

* lets the simulation run on its own. The user can also specify the number of cycles to run before pausing. If there are no processes in the ready queue that are waiting to be scheduled, EXE will return to the command interface.

**RESET** 

* allows the user to manually reset the simulator. All unfinished processes are terminated and the simulator clock returns to zero.

**EXIT** 

* allows the user to end and exit the simulator.

**HELP**

* Shows list of all commands and a short description of what each command does.  For more info on a specific command, type HELP <command-name.

**LOAD** 

* loads a program or job file into the simulator, and will also include the allocation of the program’s PCB and memory space.

**STOP**

* Stops the simulations from running when the exe command is executing.  Simulation will remain in the state in which it was stopped in.  Processes can be resumed by running the exe command again.

**SETTINGS**

* Allows user to change some general settings for the simulation
	
	**SETTINGS SPEED <1-10>** - sets the speed of the simulation
		Inputs: 1-10, 1=slowest 10=fastest
		Default: 8
	
	**SETTINGS MEMORY <1 - any int>** - sets the size of the memory used by the operating                             system.  If programs are loaded when this setting is changed the simulation will automatically reset.  
		Inputs: Any int > 0
		Default: 256
	
	**SETTINGS DECREMENTALLIO <true/false>** - sets if the io queue decrements all of its items every cycle or only decrement the top of the queue.
	Inputs: true/false
	Default: true
	
	**SETTINGS SCHEDULING <RoundRobin/FirstComeFirstServe/ShortestJobFirst>** - sets the scheduling algorithm used by the scheduler.  When this setting is changed the simulator will reset
	Inputs: RoundRobin, FirstComeFirstServe, ShortestJobFirst
	Default: RoundRobin

**GRAPH**

* opens a separate window and shows the simulation’s current memory usage on a bar chart as well as a line graph of the average waiting time of processes in the simulation V. Total completed cycles of the simulation.
