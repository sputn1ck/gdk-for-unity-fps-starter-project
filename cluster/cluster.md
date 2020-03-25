# Donnerlab cluster

1. Download and install Docker Desktop https://hub.docker.com/editions/community/docker-ce-desktop-windows
    1.1 (optional) Download a GUI
        - https://dockstation.io/
        - https://github.com/docker/kitematic/releases/download/v0.17.11/Kitematic-0.17.11-Windows.zip

2. Download and install Polar https://www.dropbox.com/s/71cxxg67pyvdytf/polar-win-v0.2.1.exe?dl=0

3. Create a new Network in Polar with 1 Bitcoind and 3 Lnd nodes
    - Alice will be Platform
    - Bob will be Daemon/Game
    - Charlie will be Gameserver/Backend

4. Open a shell (powershell or command prompt)
    - type in `docker login docker.pkg.github.com`
    - user your github username and a github access token with read:packages permission as password https://github.com/settings/tokens
    - pull images:
        - `docker pull docker.pkg.github.com/donnerlab1/bbh-backend/bbh-backend:master`
        - `docker pull docker.pkg.github.com/donnerlab1/platform-v2/platform-v2:master`

6. Adjust Docker compose located in `bountyhunt/cluster/docker-compose.yml`
    - bbh-backend:
        - adjust lnd connect, keep name carol and port, but take the rest of the polar lnd connect string
    - platform:
        - adjust lnd connect, keep name alice and port, but take the rest of the polar lnd connect string
    - network:
        - find out the network of your lnd nodes and change the name to it

7. Run Docker compose using gui or shell
    - bbh-backend might need a restart

8. Download the most recent Daemon https://github.com/donnerlab1/daemon/actions
    - select most recent master workflow -> arifacts -> daemon_win

9. Copy daemon to your existing daemon folder.

10. Adjust dd.conf.yml https://gist.github.com/sputn1ck/7bc200f983f003da0ec6f5e0a52af108
    - lndconnect: POLAR lndconnect string of bob
    - connectpeers POLAR P2P External string of alice
    - Platform POLAR pubkey of alice

11. UNITY Copy Development-POlar-kon scene

12. Adjust:
    - PolarGameLogicWorker
        - ServerServiceConnections
            - Platform Pubkey: Alice Pubkey
            - Lnd Connect: Carol Lnd Connect
    - PolarClientWorkerLnd
        - PlayerServiceConnections
            - LndConnectString: Bob Lnd Connect


