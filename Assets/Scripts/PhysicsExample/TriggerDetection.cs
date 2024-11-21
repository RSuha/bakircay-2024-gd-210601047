using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetection : MonoBehaviour
{
    private List<Collider> detectedObjects = new List<Collider>();
    private float detectionWaitTime = 3f; // Bekleme süresi
    private bool isChecking = false;
    private Vector3 ejectDirection = new Vector3(0f, 0.5f, 1f).normalized; // Hafif eðimli ileri yön
    private float ejectForce = 10f; // Dýþarý atma kuvveti

    private void OnTriggerEnter(Collider other)
    {
        // Sadece Matchable tag'ine sahip nesneler iþlenir.
        if (IsMatchableTag(other.tag))
        {
            detectedObjects.Add(other);

            // Eðer zaten kontrol eden bir coroutine yoksa baþlat.
            if (!isChecking)
            {
                StartCoroutine(CheckForMatches());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Nesne tetikleme alanýndan çýkarsa listeden kaldýrýlýr.
        if (detectedObjects.Contains(other))
        {
            detectedObjects.Remove(other);
        }
    }

    private IEnumerator CheckForMatches()
    {
        isChecking = true;

        // Bekleme süresi boyunca baþka bir nesne algýlanýrsa kontrol yapýlýr.
        yield return new WaitForSeconds(detectionWaitTime);

        // Eðer en az iki nesne varsa, eþleþme kontrolü yapýlýr.
        if (detectedObjects.Count >= 2)
        {
            var firstObject = detectedObjects[0];
            var secondObject = detectedObjects[1];

            if (AreObjectsMatching(firstObject, secondObject))
            {
                // Eþleþen nesneleri yok et.
                Destroy(firstObject.gameObject);
                Destroy(secondObject.gameObject);

                // Listeyi temizle.
                detectedObjects.Clear();
            }
            else
            {
                // Eðer eþleþme yoksa, ikinci nesneyi dýþarý at.
                EjectObject(secondObject);
                detectedObjects.Remove(secondObject);
            }
        }

        isChecking = false;
    }

    private void EjectObject(Collider obj)
    {
        // Dýþarý atmak için fizik kuvveti uygular.
        Rigidbody rb = obj.attachedRigidbody;
        if (rb != null)
        {
            rb.isKinematic = false; // Fizik etkileþimini aç
            rb.AddForce(ejectDirection * ejectForce, ForceMode.Impulse);
        }
        else
        {
            // Eðer Rigidbody yoksa pozisyonu doðrudan güncelle.
            obj.transform.position += ejectDirection * ejectForce;
        }
    }

    private bool AreObjectsMatching(Collider first, Collider second)
    {
        // Tag'lerin eþleþmesi durumunda true döner.
        return first.tag == second.tag;
    }

    private bool IsMatchableTag(string tag)
    {
        // Sadece belirli tag'ler iþlenir.
        return tag == "cubeMatchable" || tag == "circleMatchable" || tag == "capsuleMatchable";
    }
}
