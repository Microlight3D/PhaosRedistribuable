
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

rect_drawn = RBA::DBox::new(cell.bbox.left*dbu, cell.bbox.bottom*dbu, cell.bbox.right*dbu, cell.bbox.top*dbu)

# select full hierarchy and the rectangle to draw
view.max_hier
view.zoom_box(rect_drawn)

# load B&W layer properties
view.load_layer_props(layer_color)

# save image
view.save_image("#{output_file}", width, height)
