# key variables
x0=-stitchingData[2]/2
y0=-stitchingData[3]/2
stitchingPos=[1,1,stitchingData[0],stitchingData[1],1]
motionDirection=[0,0]
totImgNb = stitchingData[0] * stitchingData[1]

# load the layout
app = RBA::Application.instance
mw = app.main_window
mw.load_layout(input_file, 0)
view = mw.current_view
cv = view.cellview(view.active_cellview_index)

# calculate the drawing size
if !cv.is_valid?
  return
end

cell = cv.cell
dbu = cv.layout.dbu

# select full hierarchy
view.max_hier
  
# load B&W layer properties
view.load_layer_props(layer_color)

# hide all unselected layers
i = 0
j = 0
li = view.begin_layers
while !li.at_end?

  lp = li.current
  
  if selecIdx[j] != i
	new_lp = lp.dup
	new_lp.visible = false
	view.set_layer_properties(li, new_lp)
  elsif j + 1 < selecIdx.length
	j += 1
  end
  i += 1
  li.next

end

for i in 1..totImgNb
  reversePosX = stitchingPos[2] + 1 - stitchingPos[0]
  reversePosY = stitchingPos[1]
  if stitchingPos[1] % 2 != 0
    if stitchingPos[0] == stitchingPos[2]
      stitchingPos[1]+=1
      motionDirection = [0, 1]
    else
      stitchingPos[0]+=1
      motionDirection = [1, 0]
    end
  else
    if stitchingPos[0] == 1
      stitchingPos[1]+=1
      motionDirection = [0, 1]
    else
      stitchingPos[0]-=1
      motionDirection = [-1, 0]
    end
  end
  stitchingPos[4]++

  tempX = (reversePosX - 1) * cutWVec + x0
  tempY = (reversePosY - 1) * cutHVec + y0

  leftCoor = tempX + cell.bbox.left*dbu
  bottomCoor = tempY + cell.bbox.bottom*dbu
  rightCoor = leftCoor + cutWVec
  topCoor = bottomCoor + cutHVec
  rect_drawn = RBA::DBox::new(leftCoor, bottomCoor, rightCoor, topCoor)
  
  # select the rectangle to draw
  view.zoom_box(rect_drawn)
  
  # save image
  output_file = output_file_base + i.to_s + ".png"
  view.save_image("#{output_file}", cutW, cutH)

  #progress = i * 100 / totImgNb
  #File.write(output_file_base + "progress.txt", progress.to_s) 
end