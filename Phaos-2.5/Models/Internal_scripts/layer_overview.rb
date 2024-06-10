
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

leftCoor = leftCoor + cell.bbox.left*dbu
bottomCoor = bottomCoor + cell.bbox.bottom*dbu
rightCoor = leftCoor + cutWVec
topCoor = bottomCoor + cutHVec
rect_drawn = RBA::DBox::new(leftCoor, bottomCoor, rightCoor, topCoor)

# select full hierarchy and the rectangle to draw
view.max_hier
view.zoom_box(rect_drawn)

# load B&W layer properties
view.load_layer_props(layer_color)

# hide all layers
li = view.begin_layers
while !li.at_end?

  lp = li.current
  new_lp = lp.dup
  new_lp.visible = false
  view.set_layer_properties(li, new_lp)

  li.next

end

li = view.begin_layers
iter = 0
open("#{output_file}" + 'layer_list.txt', 'w') { |f|
while !li.at_end?

  lp = li.current
  f << lp.source_layer << "\t" << lp.source_datatype << "\n"
  new_lp = lp.dup
  new_lp.visible = true
  view.set_layer_properties(li, new_lp)
  view.save_image("#{output_file}" + iter.to_s + '.png', cutW, cutH)
  iter += 1
  new_lp = lp.dup
  new_lp.visible = false
  view.set_layer_properties(li, new_lp)
  li.next

end
}
