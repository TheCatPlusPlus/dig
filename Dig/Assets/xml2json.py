import json, sys
from xml.dom import minidom
from PIL import Image

try:
	xml_name = sys.argv[1] + '.xml'
	json_name = sys.argv[1] + '.json'
	texture = sys.argv[1] + '.png'
except IndexError:
	print('Usage: cv name')
	sys.exit(1)

xml = minidom.parse(xml_name)
root = xml.documentElement
atlas = []
texture = Image.open(texture)

def uv(u, v):
	return { 'X': u / texture.width, 'Y': v / texture.height }

for subtexture in root.getElementsByTagName('SubTexture'):
	name = subtexture.attributes['name'].value
	x = int(subtexture.attributes['x'].value)
	y = int(subtexture.attributes['y'].value)
	width = int(subtexture.attributes['width'].value)
	height = int(subtexture.attributes['height'].value)

	name = name.replace('.png', '')
	index = len(atlas)

	uv_top_left = uv(x, y)
	uv_top_right = uv(x + width, y)
	uv_bottom_left = uv(x, y + height)
	uv_bottom_right = uv(x + width, y + height)

	uv_rect = {
		'TopLeft': uv_top_left,
		'TopRight': uv_top_right,
		'BottomLeft': uv_bottom_left,
		'BottomRight': uv_bottom_right,
	}

	item = {
		'Index': index,
		'Name': name,
		'UVRect': uv_rect,

		'X': x,
		'Y': y,
		'Width': width,
		'Height': height,
	}

	atlas.append(item)

with open(json_name, 'w', encoding = 'utf-8') as fp:
	json.dump(atlas, fp, indent = 4, sort_keys = True)
