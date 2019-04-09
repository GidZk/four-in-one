All prefabs in this folder will automatically be registered to the ClientScene
so that they can be Spawn()'d (i.e. they can be instantiated across all clients
from the server). 

Note: prefabs here must have a NetworkIdentity component, otherwise it borks