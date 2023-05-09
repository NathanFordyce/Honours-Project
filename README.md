# Honours-Project

Requires Unity 2020.3.26 and the ML Agents 2.2.1-exp.1 package if not already included.


The "Demo Scene" is the same scene which is within the .exe on MLS and can be run within the editor also.


If you wish to train an agent it will require the "venv" folder that can be found on the MLS submission.
	- The venv should be saved and unzipped in the same directory as the unity project.
	- Now within command line locate to the unity project directory
	- For example "cd C:\Users\abc\Desktop\Honours-Project"
	- Now activate the venv using "venv\Scripts\activate"
	- Command line should now say "(venv) C:\Users\abc\Desktop\Honours-Project"
	- Open one of the training scenes
	- Open the corresponding environments prefab and select the agent
	- Ensure the "Behaviour Type" is set to default and "Model" is None
	- On command line start the training using "mlagents-learn config/Movement.yaml --run-id=[Name of Choice]"
	- If on the shooting environment use config/Shooting.yaml

----------------

The brains being displayed in the Demo Scene can be found in the "Final Brains" folder and are called:
	- "1 Kbm Shoot"
	- "1 Con Shoot"
	- "1 Kbm Move"
	- "1 Con Move"
	- "3 Kbm Walls"
	- "3 Con Walls"

Every trained brain can be found both within the above folder and the "results" folder in the directory