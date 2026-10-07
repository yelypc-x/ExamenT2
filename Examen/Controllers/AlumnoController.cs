using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;
using Examen.Models;

namespace Examen.Controllers
{
    public class AlumnoController : Controller
    {
        private List<Alumno> Deserializar()
        {
            string ruta = Server.MapPath("~/alumnos.json");

            string json = System.IO.File.ReadAllText(ruta);

            List<Alumno> alumnos =
                JsonConvert.DeserializeObject<List<Alumno>>(json);

            return alumnos;
        }

        private void Serializar(List<Alumno> alumnos)
        {
            string ruta = Server.MapPath("~/alumnos.json");

            string json = JsonConvert.SerializeObject(alumnos);

            System.IO.File.WriteAllText(ruta, json);
        }

        public ActionResult Index()
        {
            List<Alumno> alumnos = Deserializar();

            return View(alumnos);
        }

        [HttpGet]
        public ActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(
            string dni,
            string nombres,
            string apellidos,
            string carrera,
            int ciclo)
        {
            List<Alumno> alumnos = Deserializar();

            if (alumnos.Any(a => a.dni == dni))
            {
                ViewBag.Mensaje = "El DNI ya existe.";
                return View();
            }

            Alumno alumno = new Alumno(
                dni,
                nombres,
                apellidos,
                carrera,
                ciclo
            );

            alumnos.Add(alumno);

            Serializar(alumnos);

            TempData["Mensaje"] = "Alumno agregado correctamente.";

            return RedirectToAction("Index");
        }

        public ActionResult Eliminar(string dni)
        {
            List<Alumno> alumnos = Deserializar();

            Alumno alumno = alumnos.FirstOrDefault(a => a.dni == dni);

            if (alumno != null)
            {
                alumnos.Remove(alumno);
                Serializar(alumnos);
            }

            return RedirectToAction("Index");
        }

        // GET: Alumno/Detalles
        public ActionResult Detalles(string dni)
        {
            List<Alumno> alumnos = Deserializar();

            Alumno alumno = alumnos.FirstOrDefault(a => a.dni == dni);

            if (alumno == null)
            {
                return HttpNotFound();
            }

            return View(alumno);
        }

        // GET: Alumno/Actualizar
        [HttpGet]
        public ActionResult Actualizar(string dni)
        {
            List<Alumno> alumnos = Deserializar();

            Alumno alumno = alumnos.FirstOrDefault(a => a.dni == dni);

            if (alumno == null)
            {
                return HttpNotFound();
            }

            return View(alumno);
        }

        // POST: Alumno/Actualizar
        [HttpPost]
        public ActionResult Actualizar(
            string dniOriginal,
            string dni,
            string nombres,
            string apellidos,
            string carrera,
            int ciclo)
        {
            List<Alumno> alumnos = Deserializar();

            Alumno alumnoExistente =
                alumnos.FirstOrDefault(a => a.dni == dniOriginal);

            if (alumnoExistente == null)
            {
                return HttpNotFound();
            }

            bool dniRepetido = alumnos.Any(
                a => a.dni == dni &&
                     a.dni != dniOriginal
            );

            if (dniRepetido)
            {
                ViewBag.Mensaje = "El DNI ingresado ya pertenece a otro alumno.";

                Alumno alumnoError = new Alumno(
                    dni,
                    nombres,
                    apellidos,
                    carrera,
                    ciclo
                );

                return View(alumnoError);
            }

            alumnoExistente.dni = dni;
            alumnoExistente.nombres = nombres;
            alumnoExistente.apellidos = apellidos;
            alumnoExistente.carrera = carrera;
            alumnoExistente.ciclo = ciclo;

            Serializar(alumnos);

            TempData["Mensaje"] = "Alumno actualizado correctamente.";

            return RedirectToAction("Index");
        }
    }
}