import pydot
import os
import re
from neo4j import GraphDatabase
from dotenv import load_dotenv

# Load the environment variables
load_dotenv()

# Load the .dot file and create a graph
dot_file_path = 'src/stargripcorp.dataplatform.infra.azure/graph.dot'
graphs = pydot.graph_from_dot_file(dot_file_path)
if graphs is None:
    raise ValueError('Invalid .dot file')

graph = graphs[0]

# Function to parse the labels
def parse_label(label):
    label = label.replace('"', '')  # Remove quotes from the label
    pattern = re.compile(
        r'^urn:pulumi:(?P<stack>[^:]+)::[^:]+::pkg:(?P<pkg>[^:]+):(?P<type>[^$]+)\$(?P<subtype>[^:]+):(?P<name>.+)$'
    )
    match = pattern.match(label)
    if match:
        return match.groupdict()
    return None  # Return None if regex match fails

# Updating the traversal code to parse the labels
nodes, relationships = {}, []
for edge in graph.get_edges():
    relationships.append((edge.get_source(), edge.get_destination()))
for node in graph.get_nodes():
    node_name = node.get_name()
    label = node.get_label() if node.get_label() else node_name  # Default to node name if label is absent
    parsed_label = parse_label(label)
    if parsed_label:
        nodes[node_name] = parsed_label
    else:
        nodes[node_name] = {'name': node_name}  # Default to node name if parsing fails

# URI examples: 'neo4j://localhost', 'neo4j+s://xxx.databases.neo4j.io'
URI = os.getenv('NEO4J_URI')
user_name = os.getenv('NEO4J_USER')
password = os.getenv('NEO4J_PASSWORD')
AUTH =(user_name,password)

if URI is None:
    raise ValueError('Invalid URI')



def upload_graph(tx, nodes, relationships):
    # Create nodes, with parsed label information
    for node_name, label_info in nodes.items():
        
        n_name=label_info.get('name', node_name)
        n_type=label_info.get('type', 'Unknown')
        n_stack=label_info.get('stack','Unknown')
        n_pkg=label_info.get('pkg', 'Unknown')
        n_subtype=label_info.get('subtype', 'Unknown')
        
        if n_name != node_name:
            tx.run(
            """
                MERGE (:PulumiNode {
                    id: $id,
                    name: $name, 
                    type: $type, 
                    stack: $stack,
                    pkg: $pkg,
                    subtype: $subtype
                })
                """, 
                id=node_name,  # node_name serves as the id
                name=n_name,
                type=n_type,
                stack=n_stack,
                pkg=n_pkg,
                subtype=n_subtype
            )
    # Create relationships
    for source, destination in relationships:
        tx.run(
            '''
            MATCH (a:PulumiNode {id: $source}), (b:PulumiNode {id: $destination})
            MERGE (a)-[:DEPENDS_ON]->(b)
            ''',
            source=source, destination=destination
        )

# Invoke the script
with GraphDatabase.driver(URI, auth=AUTH) as driver:
    with driver.session() as session:
        session.execute_write(upload_graph, nodes, relationships)
