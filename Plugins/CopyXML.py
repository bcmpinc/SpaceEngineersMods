import glob
import xml.etree.ElementTree as ET
import os
import subprocess

files = glob.glob("../Content/*/*.xml") + glob.glob("*/*.xml")

def git_revision_hash(repo) -> str:
    """https://stackoverflow.com/a/21901260"""
    return subprocess.check_output(['git', '--git-dir=' + path + "/.git", 'rev-parse', 'HEAD']).decode('ascii').strip()

for f in files:
    basename = os.path.basename(f)
    print("Copying", basename)
    tree = ET.parse(f)
    node = tree.find(".//Commit")
    if node != None:
        path = os.path.dirname(f)
        node.text = git_revision_hash(path)
    tree.write("PluginHub/Plugins/" + basename)
    
