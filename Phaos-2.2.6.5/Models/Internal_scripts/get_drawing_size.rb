

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

# save drawing size
text = "#{cell.bbox.width*dbu}\t"
text += "#{cell.bbox.height*dbu}"

open(output_file, 'w') { |f|
  f.puts text
}
