import fs from 'node:fs/promises';
import path from 'node:path';
import {fileURLToPath,pathToFileURL} from 'node:url';
import {Presentation,PresentationFile} from 'file:///C:/Users/pcdan/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/@oai/artifact-tool/dist/artifact_tool.mjs';
const local=path.dirname(fileURLToPath(import.meta.url)), out=path.dirname(local);
const spec=JSON.parse(await fs.readFile(path.join(local,'slides.json'),'utf8'));
const pres=Presentation.create({slideSize:{width:1280,height:720}});
function text(s,t,x,y,w,h,size,color='#17394B',bold=false){
 const box=s.shapes.add({geometry:'textbox',position:{left:x,top:y,width:w,height:h},fill:'none',line:{fill:'none',width:0}});
 box.text=t;box.text.style={fontFamily:'Aptos',fontSize:size,color,bold};return box;
}
await fs.mkdir(path.join(local,'slides'),{recursive:true});
for(let i=0;i<spec.length;i++){
 const d=spec[i],s=pres.slides.add();s.background.fill=i===0?'#17394B':'#F7F8FA';
 const ink=i===0?'#FFFFFF':'#17394B';
 text(s,d.title,70,i===0?120:57,1120,i===0?130:110,i===0?64:46,ink,true);
 if(d.image){
  for(let j=0;j<d.lines.length;j++)text(s,d.lines[j],70,220+j*82,450,70,28,'#17394B');
  const imagePath=path.join(out,'..','04_SEGUIMIENTO',d.image);
  const data=await fs.readFile(imagePath);
  s.images.add({blob:data.buffer.slice(data.byteOffset,data.byteOffset+data.byteLength),contentType:'image/png',alt:'Pantalla de revisión de OT del 20 de octubre de 2026',fit:'contain',position:{left:560,top:210,width:650,height:380}});
  text(s,'Captura del 20 de octubre de 2026. Lista vacía de revisión.',560,610,650,35,18,'#536775');
 }else{
  for(let j=0;j<d.lines.length;j++)text(s,d.lines[j],74,(i===0?310:220)+j*(i===0?85:88),1100,75,i===0?34:32,i===0?'#D2E4EA':'#253F4A');
 }
 text(s,`${i+1} / ${spec.length}`,1145,667,90,27,17,i===0?'#D2E4EA':'#536775');
 s.speakerNotes.text=d.note;
 const png=await pres.export({slide:s,format:'png',scale:1});
 await fs.writeFile(path.join(local,'slides',`slide-${String(i+1).padStart(2,'0')}.png`),new Uint8Array(await png.arrayBuffer()));
 console.log('Render',i+1);
}
const candidate=path.join(local,'candidate.pptx');await(await PresentationFile.exportPptx(pres)).save(candidate);
const skill='C:/Users/pcdan/.codex/plugins/cache/openai-primary-runtime/presentations/26.1020.11127/skills/presentations';
const {finalizePresentation}=await import(pathToFileURL(path.join(skill,'container_tools/artifact_tool_utils.mjs')).href);
await fs.mkdir(path.join(local,'exportado'),{recursive:true});
const final=path.join(local,'exportado','Presentacion_Avance_AFRISAN_2026_10_22.pptx');
await finalizePresentation({workspaceDir:out,candidatePath:candidate,finalPath:final,
 pythonExecutable:'C:/Users/pcdan/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe',
 integrityValidatorPath:path.join(skill,'container_tools/inspect_presentation_package_integrity.py'),
 layoutValidatorPath:path.join(skill,'container_tools/inspect_presentation_layout_geometry.py'),
 layoutArgs:['--expected-slide-size-emu','12192000,6858000','--validate-heading-fit'],
 fontPolicy:{basis:'design',families:['Aptos']},verifyArtifactToolImport:true,
 receiptPath:path.join(local,'validation.json')});
await fs.copyFile(final,path.join(out,path.basename(final)));
console.log(final);
